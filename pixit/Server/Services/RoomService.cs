using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Hubs;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace pixit.Server.Services
{
    public class RoomService
    {
        private readonly IRedisDatabase _rooms;
        private readonly IHubContext<RoomHub> _hub;

        public RoomService(IRedisCacheClient redisCache, IHubContext<RoomHub> hub)
        {
            _rooms = redisCache.GetDbFromConfiguration();
            _hub = hub;
            _rooms.FlushDbAsync();
        }

        public async Task<CreateRoomEvent> Create(CreateRoomEvent roomData)
        {
            RoomModel room = new RoomModel(roomData.Name);
            string roomId = Guid.NewGuid().ToString();
            await _rooms.AddAsync(roomId, room);
            return new CreateRoomEvent()
            {
                Id = roomId
            };
        }

        public async Task<IDictionary<string,LobbyListEvent>> GetRooms()
        {
            IEnumerable<string> keys = await _rooms.SearchKeysAsync("*");
            return await _rooms.GetAllAsync<LobbyListEvent>(keys);
        }

        
        public async Task<RoomModel> Get(string roomId)
        {
            return await _rooms.GetAsync<RoomModel>(roomId);
        }

        public async Task Remove(string roomId)
        {
            await _rooms.RemoveAsync(roomId);
        }
        

        public async Task Save(string roomId, RoomModel room)
        {
            await _rooms.AddAsync(roomId, room);
        }


        public async Task<JoinRoomEvent> JoinRoom(string roomId, UserModel user, string connectionid)
        {
            RoomModel room = await Get(roomId);
            if (room == null || room.Settings.Slots <= room.UsersOnline) return null;
            user.Token = connectionid;
            user.Id = Guid.NewGuid().ToString();
            room.Users.Add(user);
            await Save(roomId, room);
            JoinRoomEvent jre = room.Adapt<JoinRoomEvent>();
            jre.Token = user.Token;
            jre.RoomId = roomId;
            jre.UserId = user.Id;
            return jre;
        }
        
        
        public async Task<UserLeftRoomEvent> LeaveRoom(string roomId, string connectionid)
        {
            RoomModel room = await Get(roomId);
            var user = room.Users.Find(u => u.Token == connectionid);
            room.Users.RemoveAll(u => u.Token == connectionid);
            if (room.UsersOnline == 0)
            {
                await Remove(roomId);
                return null;
            }
            
            await Save(roomId, room);
            if (room.Settings.HostToken == user?.Token) await SetRoomHost(roomId);
            return new UserLeftRoomEvent(user?.Id);
        }

        public async Task SetRoomHost(string roomId)
        {
            RoomModel room = await Get(roomId);
            room.HostId = room.Users[0]?.Id;
            room.Settings.HostToken = room.Users[0]?.Token;
            await Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("SetRoomHost", new SetRoomHostEvent(room.HostId));
        }

        public async Task UpdateSettings(string roomId, SettingsModel settings, string connectionid)
        {
            RoomModel room = await Get(roomId);
            if (room.Settings.HostToken != connectionid) return;
            settings.HostToken = room.Settings.HostToken;
            room.Settings = settings;
            await Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateRoomSettings", room.Settings);
        }
        
        
        public async Task<string> KickUser(string roomId, KickUserEvent user, string connectionid)
        {
            RoomModel room = await Get(roomId);
            if (room.Settings.HostToken != connectionid) return null;
            return room.Users.Find(e => e.Id == user.UserId)?.Token;
        }
    }
}
