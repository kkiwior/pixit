﻿using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        [Inject] private IJSRuntime JSRuntime { get; set; }
        
        private string Token { get; set; }
        private RoomModel RoomInfo { get; set; }
        private UserModel User { get; set; }

        protected override async Task OnInitializedAsync()
        {
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

            RoomInfo = State?.Room;
            StateHasChanged();

            await Mediator.Register<JoinRoomEvent>(HandleJoinRoom);
            await Mediator.Register<SetRoomHostEvent>(HandleSetRoomHost);
            await Mediator.Register<UserJoinedRoomEvent>(HandleUserJoined);
            await Mediator.Register<UserLeftRoomEvent>(HandleUserLeft);
            await Mediator.Register<SettingsModel>(HandleSettingsUpdate);
            await Mediator.Register<KickUserEvent>(HandleKick);
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
            await LocalStorage.SetItemAsync("token", jre.Token);
            Token = jre.Token;
            RoomInfo = new RoomModel(jre.Name)
            {
                Settings = jre.Settings,
                Users = jre.Users,
                Started = jre.Started,
                HostId = jre.HostId
            };
            State.UserId = jre.UserId;
            StateHasChanged();
        }

        private Task HandleSetRoomHost(SetRoomHostEvent e)
        {
            RoomInfo.HostId = e.HostId;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task HandleSettingsUpdate(SettingsModel settings)
        {
            RoomInfo.Settings = settings;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task HandleUserJoined(UserJoinedRoomEvent user)
        {
            RoomInfo.Users.Add(user.Adapt<UserModel>());
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        protected Task HandleUserLeft(UserLeftRoomEvent user)
        {
            RoomInfo.Users.RemoveAll(u=>u.Id == user.Id);
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected async Task UpdateSettings()
        {
            if(RoomInfo.Settings.Slots >= RoomInfo.Users.Count && RoomInfo.Settings.Slots <= 20 && RoomInfo.Settings.MaxScore >= 5 && RoomInfo.Settings.MaxScore <= 100)
                await Event.UpdateSettings(RoomInfo.Settings);
        }

        public void Dispose()
        {
            if (RoomInfo?.Name != null)
            {
                Event.UserLeftRoom(new UserLeftRoomEvent
                {
                    Token = Token,
                    RoomId = RoomId
                });               
            }
            Mediator.Unregister<JoinRoomEvent>();
            Mediator.Unregister<SetRoomHostEvent>();
            Mediator.Unregister<UserJoinedRoomEvent>();
            Mediator.Unregister<UserLeftRoomEvent>();
            Mediator.Unregister<SettingsModel>();
            Mediator.Unregister<KickUserEvent>();
        }

        private void KickUser(string userId)
        {
            if (userId == State.UserId) return;
            Event.KickUser(new KickUserEvent()
            {
                UserId = userId
            });
        }
    }
}