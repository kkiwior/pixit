using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Client.Shared;
using pixit.Client.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Client.Pages
{
    public partial class Lobby : IDisposable
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private Mediator Mediator { get; set; }
        [Inject] private SendEventService Event  { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private Modal _modal = new();
        private UserModel User;

        private CreateRoomModel CreateRoomForm = new();
        Dictionary<string, RoomModel> _roomList = new();
        
        protected override async Task OnInitializedAsync()
        {
            User = await LocalStorage.GetItemAsync<UserModel>("user");
            if (User == null) Navigation.NavigateTo("");

            await Mediator.Register<Dictionary<string, RoomModel>>(HandleGetRooms);
            await Mediator.Register<KeyValuePair<string, RoomModel>>(HandleGetRoom);
            await Mediator.Register<JoinRoomEvent>(HandleJoinRoom);
        }

        protected async Task HandleJoinRoom(JoinRoomEvent room)
        {
            await LocalStorage.SetItemAsync("token", room.Token);
            Navigation.NavigateTo("/room/" + room.RoomId);
        }

        public void Dispose()
        {
            Mediator.Unregister<Dictionary<string, RoomModel>>();
            Mediator.Unregister<KeyValuePair<string, RoomModel>>();
        }

        protected Task HandleGetRooms(Dictionary<string, RoomModel> rooms)
        {
            _roomList = rooms;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task HandleGetRoom(KeyValuePair<string, RoomModel> room)
        {
            _roomList.Add(room.Key, room.Value);
            StateHasChanged();
            return Task.CompletedTask;
        } 

        private void CreateRoomDialog() => _modal.Toggle();
        private async void CreateRoom()
        {
            Validator.TryValidateObject(User, new ValidationContext(User), null, true);
            await LocalStorage.SetItemAsync<UserModel>("user", User);
            CreateRoomForm.User = User;
            await Event.CreateRoom(CreateRoomForm);
            _modal.Toggle();
        }

        private async void JoinRoom(string id)
        {
            await Event.UserJoinRoom(new JoinRoomEvent()
            {
                RoomId = id,
                User = User
            });
        }
    }
}