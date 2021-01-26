using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Shared.Models;

namespace pixit.Server.Hubs
{
    public class RoomHub : Hub
    {
        private readonly RoomService _rooms;

        public RoomHub(RoomService roomService)
        {
            _rooms = roomService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Rooms");
            await this.SendRooms();
        }

        public async Task SendRooms()
        {
            await Clients.Caller.SendAsync("SendRooms", _rooms.GetRooms());
        }

        public async Task CreateRoom(RoomModel roomData)
        {
            var room = await _rooms.Create(roomData);
            await Clients.Group("Rooms").SendAsync("SendRoom", room);
        }
    }
}