using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
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
            JoinRoomEvent jre = await _rooms.Create(roomData);
            await JoinRoom(jre.RoomId, jre.User);
        }
        

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            await JoinRoom(data.RoomId, data.User);
        }
        
        
        public async Task JoinRoom(string roomId, UserModel User)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "rooms");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoinedRoom", User);
            await Clients.Caller.SendAsync("JoinRoomEvent", await _rooms.JoinRoom(roomId, User));
        }    
        
        
        public async Task UserLeftRoom(UserLeftRoomEvent data)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, data.RoomId);
            await Clients.Group(data.RoomId).SendAsync("UserLeftRoom", await _rooms.LeaveRoom(data.RoomId, data.Token));
        }
    }
}