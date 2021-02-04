using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http.Features;
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

        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            if (Context.Items.ContainsKey("room"))
            {
                UserLeftRoom(new UserLeftRoomEvent()
                {
                    RoomId = Context.Items["room"].ToString(),
                    Token = Context.ConnectionId
                });
            }
        }

        
        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
            await Clients.Caller.SendAsync("SendRooms", await _rooms.GetRooms());
        }

        public async Task SendRoom(string roomId)
        {
            RoomModel room = await _rooms.Get(roomId);
            await Clients.Group("lobby").SendAsync("SendRoom", new KeyValuePair<string, LobbyListEvent>(roomId, room.Adapt<LobbyListEvent>()));
        }

        
        public async Task CreateRoom(CreateRoomModel roomData)
        {
            JoinRoomEvent jre = await _rooms.Create(roomData, Context.ConnectionId);
            await JoinRoom(jre.RoomId, jre.User);
        }
        

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            await JoinRoom(data.RoomId, data.User);
        }
        
        
        public async Task JoinRoom(string roomId, UserModel User)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "lobby");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            User.Id = Guid.NewGuid().ToString();
            JoinRoomEvent jre = await _rooms.JoinRoom(roomId, User, Context.ConnectionId);
            if (jre == null) return;
            await Clients.Caller.SendAsync("JoinRoomEvent", jre);
            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("UserJoinedRoom", User);
            Context.Items.Add("room", roomId);
            await SendRoom(roomId);
        }    
        
        
        public async Task UserLeftRoom(UserLeftRoomEvent data)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, data.RoomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
            await Clients.Group(data.RoomId).SendAsync("UserLeftRoom", await _rooms.LeaveRoom(Context.Items["room"].ToString(), Context.ConnectionId));
            Context.Items.Remove("room");
            await SendRoom(data.RoomId);
        }

        public async Task UpdateSettings(SettingsModel settings)
        {
            await _rooms.UpdateSettings(Context.Items["room"].ToString(), settings, Context.ConnectionId);
            await SendRoom(Context.Items["room"].ToString());
        }
    }
}