using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
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
        
        private string Token { get; set; }
        private RoomInfoEvent RoomInfo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Mediator.Register<RoomInfoEvent>(HandleRoomInfo);
            await Mediator.Register<UserJoinedRoomEvent>(HandleUserJoined);
            await Mediator.Register<UserLeftRoomEvent>(HandleUserLeft);
            
            Token = await LocalStorage.GetItemAsync<string>("token");
            await Event.GetRoomInfo(new JoinRoomEvent()
            {
                RoomId = RoomId,
                Token = Token
            });
        }

        protected Task HandleUserJoined(UserJoinedRoomEvent user)
        {
            RoomInfo.Users.Add(new UserModel()
            {
                Name = user.Name,
                Avatar = user.Avatar
            });
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        protected Task HandleUserLeft(UserLeftRoomEvent user)
        {
            RoomInfo.Users.RemoveAll(u => u.Name == user.Username);
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task HandleRoomInfo(RoomInfoEvent RoomInfo)
        {
            this.RoomInfo = RoomInfo;
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
            Mediator.Unregister<RoomInfoEvent>();
            Mediator.Unregister<UserJoinedRoomEvent>();
        }
    }
}