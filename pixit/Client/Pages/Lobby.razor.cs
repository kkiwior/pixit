using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Client.Shared;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Lobby : ComponentBase
    {
        [Inject] protected Mediator Mediator { get; set; }
        [Inject] protected SendEventService Event  { get; set; }
        private Modal Modal  { get; set; }

        private CreateRoomModel CreateRoomForm = new CreateRoomModel();
        List<RoomModel> _roomList = new List<RoomModel>();

        protected override async Task OnInitializedAsync()
        {
            await Mediator.Register<UserModel>(HandleUserJoined);
            await Mediator.Register<List<RoomModel>>(HandleGetRooms);
        }

        protected Task HandleUserJoined(UserModel user)
        {
            return Task.CompletedTask;
            // tu bd jakiś token coś
        }

        protected Task HandleGetRooms(List<RoomModel> rooms)
        {
            _roomList = rooms;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void CreateRoomDialog() => Modal.Toggle();
        private void CreateRoom()
        {
            Event.CreateRoom(CreateRoomForm);
            Modal.Toggle();
        }
    }
}