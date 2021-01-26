using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Client.Shared;
using pixit.Client.Utils;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Lobby : IDisposable
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private Mediator Mediator { get; set; }
        [Inject] private SendEventService Event  { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private Modal _modal = new();

        private CreateRoomModel CreateRoomForm = new();
        Dictionary<string, RoomModel> _roomList = new();
        
        protected override async Task OnInitializedAsync()
        {
            if(String.IsNullOrEmpty(await LocalStorage.GetItemAsync<string>("username"))) Navigation.NavigateTo("");

            await Mediator.Register<Dictionary<string, RoomModel>>(HandleGetRooms);
            await Mediator.Register<KeyValuePair<string, RoomModel>>(HandleGetRoom);
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
            await Event.CreateRoom(CreateRoomForm);
            _modal.Toggle();
        }
    }
}