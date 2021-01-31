using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Mapster;
using Microsoft.AspNetCore.Components;
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
        private RoomModel RoomInfo { get; set; }
        private UserModel User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await LocalStorage.GetItemAsync<UserModel>("user");
            if (User == null) Navigation.NavigateTo("");
            
            Token = await LocalStorage.GetItemAsync<string>("token");
            RoomInfo = State.Room;
            StateHasChanged();
            
            await Mediator.Register<UserJoinedRoomEvent>(HandleUserJoined);
            await Mediator.Register<UserLeftRoomEvent>(HandleUserLeft);
            

        }

        protected Task HandleUserJoined(UserJoinedRoomEvent user)
        {
            RoomInfo.Users.Add(user.Adapt<UserModel>());
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        protected Task HandleUserLeft(UserLeftRoomEvent user)
        {
            RoomInfo.Users.RemoveAll(u => u.Id == user.Id);
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        
        public void Dispose()
        {
            Event.UserLeftRoom(new UserLeftRoomEvent()
            {
                Token = Token,
                RoomId = RoomId
            });
            Mediator.Unregister<UserJoinedRoomEvent>();
            Mediator.Unregister<UserLeftRoomEvent>();
        }
    }
}