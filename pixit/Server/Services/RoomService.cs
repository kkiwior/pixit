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

        public async Task<JoinRoomEvent> Create(CreateRoomModel roomData)
        {
            RoomModel room = roomData.Adapt<RoomModel>();
            UserModel user = roomData.User;
            string roomId = Guid.NewGuid().ToString();
            user.Token = Guid.NewGuid().ToString();
            room.Host = user.Token;
            await _rooms.AddAsync(roomId, room);
            await _hub.Clients.Group("Rooms").SendAsync("SendRoom", room);
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
        

        public async Task Save(string roomId, RoomModel room)
        {
            await _rooms.AddAsync(roomId, room);
        }


        public async Task<JoinRoomEvent> JoinRoom(string roomId, UserModel User)
        {
            RoomModel room = await Get(roomId);
            if (room == null || room.Slots <= room.UsersOnline) return null;
            if (room.UsersOnline == 0) User.Token = room.Host;
            else User.Token = Guid.NewGuid().ToString();
            room.Users.Add(User);
            await Save(roomId, room);
            JoinRoomEvent jre = room.Adapt<JoinRoomEvent>();
            jre.Token = User.Token;
            jre.RoomId = roomId;
            return jre;
        }

        
        public async Task<UserLeftRoomEvent> LeaveRoom(string roomId, string token)
        {
            RoomModel room = await Get(roomId);
            var user = room.Users.Find(u => u.Token == token);
            room.Users.RemoveAll(u => u.Token == token);
            await Save(roomId, room);
            return new UserLeftRoomEvent(user.Id);
        }
    }
}
