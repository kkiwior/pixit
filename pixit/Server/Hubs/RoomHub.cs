using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Server.Hubs
{
    public class RoomHub : GameHub
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
                    RoomId = Context.Items["room"]?.ToString(),
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

        
        public async Task CreateRoom(CreateRoomEvent roomData)
        {
            await Clients.Caller.SendAsync("CreateRoom", await _rooms.Create(roomData));
        }
        

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            await JoinRoom(data.RoomId, data.User);
        }
        

        public async Task UserLeftRoom(UserLeftRoomEvent data)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, data.RoomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
            await Clients.Group(data.RoomId).SendAsync("UserLeftRoom", await _rooms.LeaveRoom(Context.Items["room"]?.ToString(), Context.ConnectionId));
            Context.Items.Remove("room");
            await SendRoom(data.RoomId);
        }
        
        
        public async Task JoinRoom(string roomId, UserModel user)
        {
            JoinRoomEvent jre = await _rooms.JoinRoom(roomId, user, Context.ConnectionId);
            if (jre == null)
            {
                await Clients.Caller.SendAsync("JoinRoomEvent", new JoinRoomEvent()
                {
                    Started = true
                });
                return;
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "lobby");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("JoinRoomEvent", jre);
            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("UserJoinedRoom", user);
            Context.Items.Add("room", roomId);
            await SendRoom(roomId);
            if (jre.Users.Count == 1) await _rooms.SetRoomHost(roomId);
        }    
        

        public async Task UpdateSettings(SettingsModel settings)
        {
            await _rooms.UpdateSettings(Context.Items["room"]?.ToString(), settings, Context.ConnectionId);
            await SendRoom(Context.Items["room"]?.ToString());
        }


        public async Task KickUser(KickUserEvent user)
        {
            string connectionId = await _rooms.KickUser(Context.Items["room"]?.ToString(), user, Context.ConnectionId);
            await Clients.Client(connectionId).SendAsync("KickUser", user);
        }
    }
}