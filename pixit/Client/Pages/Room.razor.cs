﻿using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using pixit.Client.Services;
using pixit.Client.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Client.Pages
{
    public partial class Room : IDisposable
    {
        [Parameter] public string RoomId { get; set; }

        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private SendEventService Event { get; set; }
        [Inject] private Mediator Mediator { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private StateContainer State { get; set; }

        private string Token { get; set; }
        private UserModel User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            State.PropertyChanged += (_, _) => StateHasChanged(); 

            User = await LocalStorage.GetItemAsync<UserModel>("user");
            if (User == null)
            {
                State.JoinRoomAfterLogin = RoomId;
                Navigation.NavigateTo("");
                return;
            }
            User.Validate();

            await Event.UserJoinRoom(new JoinRoomEvent()
            {
                RoomId = RoomId,
                User = User
            });

            StateHasChanged();

            await Mediator.Register<JoinRoomEvent>(HandleJoinRoom);
            await Mediator.Register<KickUserEvent>(HandleKick);
            await Mediator.Register<GameModel>(HandleGameStart);
            Navigation.LocationChanged += LocationChanged;
        }
        
        public void Dispose()
        {
            Mediator.Unregister<JoinRoomEvent>();
            Mediator.Unregister<KickUserEvent>();
            Mediator.Unregister<GameModel>();
            Navigation.LocationChanged -= LocationChanged;
        }
        
        private Task HandleGameStart(GameModel arg)
        {
            Navigation.NavigateTo("game");
            return Task.CompletedTask;
        }

        private Task HandleKick(KickUserEvent arg)
        {
            Navigation.NavigateTo("lobby");
            return Task.CompletedTask;
        }

        protected async Task HandleJoinRoom(JoinRoomEvent jre)
        {
            if (jre.Started)
            {
                Navigation.NavigateTo("lobby");
                return;
            }
            Token = jre.Token;
            await LocalStorage.SetItemAsync("token", Token);
            State.Room = new RoomModel(jre.Name)
            {
                Settings = jre.Settings,
                Users = jre.Users,
                Started = jre.Started,
                HostId = jre.HostId
            };
            State.UserId = jre.UserId;
            StateHasChanged();
        }

        protected async Task UpdateSettings()
        {
            if (State.Room.HostId != State.UserId) return;
            if (State.Room.Settings.Slots >= State.Room.Users.Count && State.Room.Settings.Slots <= 20 && State.Room.Settings.MaxScore >= 5 && State.Room.Settings.MaxScore <= 100)
                await Event.UpdateSettings(State.Room.Settings);
        }

        private void LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (State.Room?.Name != null && !e.Location.Contains("game"))
            {
                Event.UserLeftRoom();
            }
        }

        private void KickUser(string userId)
        {
            if (userId == State.UserId) return;
            Event.KickUser(new KickUserEvent()
            {
                UserId = userId
            });
        }

        private async Task StartGame()
        {
            await Event.StartGame();
        }
    }
}