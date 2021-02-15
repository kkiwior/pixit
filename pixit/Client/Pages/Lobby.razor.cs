using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        
        //[Inject] private IJSRuntime JSRuntime { get; set; }

        private Modal _modal = new();
        private UserModel _user;

        private CreateRoomModel CreateRoomForm = new();
        Dictionary<string, LobbyListEvent> _roomList = new();
        
        protected override async Task OnInitializedAsync()
        {
            _user = await LocalStorage.GetItemAsync<UserModel>("user");

            await Mediator.Register<Dictionary<string, LobbyListEvent>>(HandleGetRooms);
            await Mediator.Register<KeyValuePair<string, LobbyListEvent>>(HandleGetRoom);
            await Mediator.Register<JoinRoomEvent>(HandleRoomCreate);
            await Event.GetRooms();
        }

        private Task HandleRoomCreate(JoinRoomEvent jre)
        {
            State.UserId = jre.UserId;
            State.Room = new RoomModel(jre.Name)
            {
                Settings = jre.Settings,
                Users = jre.Users,
                Started = jre.Started,
                HostId = jre.UserId
            };
            Navigation.NavigateTo("/room/" + jre.RoomId);
            return Task.CompletedTask;
        }

        private async void JoinRoom(string id)
        {
            Validator.TryValidateObject(_user, new ValidationContext(_user), null, true);
            await LocalStorage.SetItemAsync("user", _user);

            await Event.UserJoinRoom(new JoinRoomEvent()
            {
                RoomId = id,
                User = _user
            });
            Navigation.NavigateTo("/room/" + id);
        }

        public void Dispose()
        {
            Mediator.Unregister<Dictionary<string, LobbyListEvent>>();
            Mediator.Unregister<KeyValuePair<string, LobbyListEvent>>();
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
            Validator.TryValidateObject(_user, new ValidationContext(_user), null, true);
            await LocalStorage.SetItemAsync("user", _user);
            CreateRoomForm.User = _user;
            await Event.CreateRoom(CreateRoomForm);
            _modal.Toggle();
        }
    }
}