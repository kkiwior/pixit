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
        //private readonly ConcurrentDictionary<string, RoomModel> _rooms = new();
        private readonly IRedisDatabase _rooms;
        private readonly IHubContext<RoomHub> _hub;

        public RoomService(IRedisCacheClient redisCache, IHubContext<RoomHub> hub)
        {
            _rooms = redisCache.GetDbFromConfiguration();
            _hub = hub;
        }

        public async Task<JoinRoomEvent> Create(CreateRoomModel roomData, string connectionid)
        {
            RoomModel room = new RoomModel(roomData.Name);
            UserModel user = roomData.User;
            string roomId = Guid.NewGuid().ToString();
            user.Token = connectionid;
            await _rooms.AddAsync(roomId, room);
            return new JoinRoomEvent()
            {
                User = user,
                RoomId = roomId
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


        public async Task<JoinRoomEvent> JoinRoom(string roomId, UserModel User, string connectionid)
        {
            RoomModel room = await Get(roomId);
            if (room == null || room.Settings.Slots <= room.UsersOnline) return null;
            User.Token = connectionid;
            if (room.UsersOnline == 0) room.Settings.Host = connectionid;
            room.Users.Add(User);
            await Save(roomId, room);
            JoinRoomEvent jre = room.Adapt<JoinRoomEvent>();
            jre.Token = User.Token;
            jre.RoomId = roomId;
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
            return new UserLeftRoomEvent(user.Id);
        }
    }
}
