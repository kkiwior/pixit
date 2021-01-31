using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Client.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Client.Pages
{
    partial class Room
    {
        [Parameter] public string RoomId { get; set; }
        
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private SendEventService Event { get; set; }
        [Inject] private Mediator Mediator { get; set; }
        
        private string Token { get; set; }
        private RoomModel RoomInfo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Mediator.Register<RoomModel>(HandleRoomInfo);
            await Mediator.Register<UserModel>(HandleUserJoined);
            
            Token = await LocalStorage.GetItemAsync<string>("token");
            await Event.GetRoomInfo(new JoinRoomEvent()
            {
                RoomId = RoomId,
                Token = Token
            });
        }

        protected Task HandleUserJoined(UserModel user)
        {
            RoomInfo.Users.Add(user);
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task HandleRoomInfo(RoomModel RoomInfo)
        {
            this.RoomInfo = RoomInfo;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}