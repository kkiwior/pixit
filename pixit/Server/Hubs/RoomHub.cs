using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Repositiories;
using pixit.Server.Services;
using pixit.Server.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Server.Hubs
{
    public partial class RoomHub : RoomToHubExtension
    {
        private readonly RoomService _roomService;
        private readonly RoomRepository _rooms;
        private readonly GameService _games;


        public RoomHub(RoomService roomService, RoomRepository roomRepository, GameService gameService)
        {
            _rooms = roomRepository;
            _roomService = roomService;
            _games = gameService;
        }


        public override async Task OnConnectedAsync()
        {
            Context.Items.Add("room", null);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            try
            {
                Context.Items.TryGetValue("room", out var roomId);
                Room = await _rooms.GetRoomById(roomId?.ToString());
                await UserLeftRoom(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await base.OnDisconnectedAsync(ex);
        }

        
        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
            await Clients.Caller.SendAsync("SendRooms", await _rooms.GetAllRooms());
        }

        public async Task SendRoom(string roomId)
        {
            RoomModel room = await _rooms.GetRoomById(roomId);
            await Clients.Group("lobby").SendAsync("SendRoom", new KeyValuePair<string, LobbyListEvent>(roomId, room.Adapt<LobbyListEvent>()));
        }

        
        public async Task CreateRoom(CreateRoomEvent roomData)
        {
            await Clients.Caller.SendAsync("CreateRoom", await _roomService.Create(roomData));
        }
        

        public async Task UserJoinRoom(JoinRoomEvent data)
        {
            await JoinRoom(data.RoomId, data.User);
        }
        

        public async Task UserLeftRoom(bool disconnected = false)
        {
            await _roomService.LeaveRoom(Context.Items["room"]?.ToString(), Room, Context.ConnectionId, true);
            await SendRoom(Context.Items["room"]?.ToString());
            Context.Items["room"] = null;
        }
        
        
        public async Task JoinRoom(string roomId, UserModel user)
        {
            JoinRoomEvent jre = await _roomService.JoinRoom(roomId, user, Context.ConnectionId);
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
            Context.Items["room"] = roomId;
            await SendRoom(roomId);
            if (jre.Users.Count == 1) await _roomService.SetRoomHost(roomId, Room);
        }
        

        public async Task UpdateSettings(SettingsModel settings)
        {
            await _roomService.UpdateSettings(Context.Items["room"]?.ToString(), Room, settings, Context.ConnectionId);
            await SendRoom(Context.Items["room"]?.ToString());
        }


        public async Task KickUser(KickUserEvent user)
        {
            string connectionId = await _roomService.KickUser(Context.Items["room"]?.ToString(), Room, user, Context.ConnectionId);
            await Clients.Client(connectionId).SendAsync("KickUser", user);
        }
    }
}