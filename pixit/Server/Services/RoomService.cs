using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Hubs;
using pixit.Server.Repositiories;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace pixit.Server.Services
{
    public class RoomService
    {
        private readonly IHubContext<RoomHub> _hub;
        private readonly RoomRepository _rooms;

        public RoomService(RoomRepository rooms, IHubContext<RoomHub> hub)
        {
            _hub = hub;
            _rooms = rooms;
        }

        public async Task<CreateRoomEvent> Create(CreateRoomEvent roomData)
        {
            RoomModel room = new RoomModel(roomData.Name);
            string roomId = await _rooms.Create(room);
            return new CreateRoomEvent()
            {
                Id = roomId
            };
        }

        public async Task<JoinRoomEvent> JoinRoom(string roomId, UserModel user, string connectionid)
        {
            RoomModel room = await _rooms.GetRoomById(roomId);
            if (room == null || room.Settings.Slots <= room.UsersOnline || room.Started) return null;
            user.Token = connectionid;
            user.Id = Guid.NewGuid().ToString();
            room.Users.Add(user);
            await _rooms.Save(roomId, room);
            JoinRoomEvent jre = room.Adapt<JoinRoomEvent>();
            jre.Token = user.Token;
            jre.RoomId = roomId;
            jre.UserId = user.Id;
            return jre;
        }
        
        
        public async Task LeaveRoom(string roomId, RoomModel room, string connectionid)
        {
            if (room.UsersOnline == 1)
            {
                await _rooms.Remove(roomId);
                return;
            }
            
            UserModel user = await RemoveUserFromRoom(roomId, room, null, connectionid);
            await _rooms.Save(roomId, room);
            if (room.Settings.HostToken == user?.Token) await SetRoomHost(roomId, room);
        }

        public async Task SetRoomHost(string roomId, RoomModel room)
        {
            if (room == null) room = await _rooms.GetRoomById(roomId);
            room.HostId = room.Users[0]?.Id;
            room.Settings.HostToken = room.Users[0]?.Token;
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("SetRoomHost", new SetRoomHostEvent(room.HostId));
        }

        public async Task UpdateSettings(string roomId, RoomModel room, SettingsModel settings, string connectionid)
        {
            if (room.Settings.HostToken != connectionid) return;
            settings.HostToken = room.Settings.HostToken;
            room.Settings = settings;
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateRoomSettings", room.Settings);
        }
        
        
        public async Task<string> KickUser(string roomId, RoomModel room, KickUserEvent userToKick, string connectionid)
        {
            if (room.Settings.HostToken != connectionid) return null;
            UserModel user = await RemoveUserFromRoom(roomId, room, userToKick.UserId);
            return user.Token;
        }

        public async Task<UserModel> RemoveUserFromRoom(string roomId, RoomModel room, string userId = null, string token = null)
        {
            if (token == null)
            {
                token = await Task.FromResult(room.Users.FirstOrDefault(u => u.Id == userId)?.Token);
            }
            
            UserModel user = await Task.FromResult(room.Users.FirstOrDefault(u => u.Token == token));
            room.Users.Remove(user);
            await _hub.Groups.RemoveFromGroupAsync(token, roomId);
            await _hub.Groups.AddToGroupAsync(token, "lobby");
            await _hub.Clients.Group(roomId).SendAsync("UserLeftRoom", new UserLeftRoomEvent(user?.Id));
            return user;
        }
    }
}
