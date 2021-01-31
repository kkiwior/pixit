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
            await SendRooms();
        }

        public async Task SendRooms()
        {
            await Clients.Caller.SendAsync("SendRooms", await _rooms.GetRooms());
        }

        public async Task CreateRoom(CreateRoomModel roomData)
        {
            var room = await _rooms.Create(roomData);
            await Clients.Group("Rooms").SendAsync("SendRoom", room);
            await JoinRoom(room.Key, roomData.User);
        }

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            var res = await _rooms.JoinRoom(data.RoomId, data.User);
            if(res != null) await JoinRoom(data.RoomId, data.User);
        }
        
        public async Task UserLeftRoom(JoinRoomEvent data)
        {
            var username = await _rooms.LeaveRoom(data.RoomId, data.Token);
            await Clients.Group(data.RoomId).SendAsync("UserLeftRoom", new UserLeftRoomEvent()
            {
                Username = username
            });
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
            await Clients.Group(roomId).SendAsync("UserJoinedRoom", new UserJoinedRoomEvent()
            {
                Name = User.Name,
                Avatar = User.Avatar
            });
        }

        public async Task GetRoomInfo(JoinRoomEvent session)
        {
            await Clients.Caller.SendAsync("RoomInfo", await _rooms.GetInfo(session.RoomId, session.Token));
        }
    }
}