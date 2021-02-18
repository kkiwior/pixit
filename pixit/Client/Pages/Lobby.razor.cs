using System;
using System.Collections.Generic;
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
        [Inject] private SendEventService Event { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private StateContainer State { get; set; }
        
        private Modal _modal = new();
        private UserModel _user;

        private readonly CreateRoomEvent _createRoomForm = new();
        Dictionary<string, LobbyListEvent> _roomList = new();

        protected override async Task OnInitializedAsync()
        {
            State.PropertyChanged += (_, _) => StateHasChanged(); 
            _user = await LocalStorage.GetItemAsync<UserModel>("user");

            await Mediator.Register<Dictionary<string, LobbyListEvent>>(HandleGetRooms);
            await Mediator.Register<KeyValuePair<string, LobbyListEvent>>(HandleGetRoom);
            await Mediator.Register<CreateRoomEvent>(HandleCreateRoom);
            await Event.GetRooms();
        }
        
        public void Dispose()
        {
            Mediator.Unregister<Dictionary<string, LobbyListEvent>>();
            Mediator.Unregister<KeyValuePair<string, LobbyListEvent>>();
            Mediator.Unregister<CreateRoomEvent>();
        }

        private Task HandleCreateRoom(CreateRoomEvent e)
        {
            Navigation.NavigateTo("/room/" + e.Id);
            return Task.CompletedTask;
        }

        private async void JoinRoom(string id)
        {
            _user.Validate();
            await LocalStorage.SetItemAsync("user", _user);
            Navigation.NavigateTo("/room/" + id);
        }
        
        protected Task HandleGetRooms(Dictionary<string, LobbyListEvent> rooms)
        {
            _roomList = rooms;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task HandleGetRoom(KeyValuePair<string, LobbyListEvent> room)
        {
            if (room.Value == null)
            {
                _roomList.Remove(room.Key);
            }
            else if (_roomList.ContainsKey(room.Key))
            {
                _roomList[room.Key] = room.Value;
            }
            else
            {
                _roomList.Add(room.Key, room.Value);
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void CreateRoomDialog() => _modal.Toggle();

        private async void CreateRoom()
        {
            _user.Validate();
            _createRoomForm.User = _user;
            await LocalStorage.SetItemAsync("user", _user);
            await Event.CreateRoom(_createRoomForm);
            _modal.Toggle();
        }
    }
}