using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

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

        public async Task CreateRoom(CreateRoomModel roomData)
        {
            var room = await _rooms.Create(roomData);
            await Clients.Group("Rooms").SendAsync("SendRoom", room);
            await JoinRoom(room.Key, room.Value.Users.First());
        }

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            data.User.Token = await _rooms.JoinRoom(data.RoomId, data.User);
            await JoinRoom(data.RoomId, data.User);
        }

        public async Task JoinRoom(string roomId, UserModel User)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "rooms");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("JoinRoomEvent", new JoinRoomEvent()
            {
                RoomId = roomId,
                Token = User.Token
            });
            await Clients.Group(roomId).SendAsync("UserJoinedRoom", User);
        }

        public async Task GetRoomInfo(JoinRoomEvent session)
        {
            RoomModel room = await _rooms.GetInfo(session.RoomId, session.Token);
            if(room != null) await Clients.Caller.SendAsync("RoomInfo", room);
        }
    }
}