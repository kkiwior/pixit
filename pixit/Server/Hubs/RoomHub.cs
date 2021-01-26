using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Shared.Models;

namespace pixit.Server.Hubs
{
    public class RoomHub : Hub
    {
        private RoomService _rooms;

        public RoomHub(RoomService RoomService)
        {
            _rooms = RoomService;
        }

        public async Task UserJoin(UserModel User)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Rooms");
            await this.GetRooms();
        }

        public async Task GetRooms()
        {
            await Clients.Caller.SendAsync("GetRooms", await _rooms.GetRooms());
        }

        public async Task CreateRoom(RoomModel roomData)
        {
            await _rooms.Create(roomData);
            await Clients.Group("Rooms").SendAsync("GetRooms", await _rooms.GetRooms());
        }
    }
}