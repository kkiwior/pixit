using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace pixit.Server.Services
{
    public class RoomService
    {
        //private readonly ConcurrentDictionary<string, RoomModel> _rooms = new();
        private readonly IRedisDatabase _rooms;

        public RoomService(IRedisCacheClient redisCache)
        {
            _rooms = redisCache.GetDbFromConfiguration();
        }

        public async Task<KeyValuePair<string, LobbyListEvent>> Create(CreateRoomModel roomData)
        {
            RoomModel room = new RoomModel()
            {
                Name = roomData.Name
            };
            UserModel user = roomData.User;
            string roomId = Guid.NewGuid().ToString();
            user.Token = Guid.NewGuid().ToString();
            room.Host = user.Token;
            room.Users.Add(user); //przeniesc
            await _rooms.AddAsync(roomId, room);
            return new KeyValuePair<string, LobbyListEvent>(roomId, await _rooms.GetAsync<LobbyListEvent>(roomId));
        }

        public async Task<IDictionary<string,LobbyListEvent>> GetRooms()
        {
            IEnumerable<string> keys = await _rooms.SearchKeysAsync("*");
            //return new Dictionary<string, LobbyListEvent>();
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
        
        public async Task<RoomInfoEvent> GetInfo(string roomId, string Token)
        {
            RoomModel room = await Get(roomId);
            if (!room.Users.Exists(u => u.Token == Token)) return null;
            return new RoomInfoEvent()
            {
                Name = room.Name,
                CardsCount = room.CardsCount,
                MaxScore = room.MaxScore,
                Slots = room.Slots,
                Started = room.Started,
                Users = room.Users
            };
        }
        
        public async Task<JoinRoomEvent> JoinRoom(string roomId, UserModel User)
        {
            RoomModel room = await this.Get(roomId);
            if (room.Slots <= room.UsersOnline) return null;
            User.Token = Guid.NewGuid().ToString();
            room.Users.Add(User);
            await Save(roomId, room);
            return new JoinRoomEvent()
            {
                RoomId = roomId,
                Token = User.Token
            };
        }

        public async Task<string> LeaveRoom(string roomId, string token)
        {
            RoomModel room = await Get(roomId);
            var user = room.Users.Find(u => u.Token == token);
            room.Users.RemoveAll(u => u.Token == token);
            await Save(roomId, room);
            return user.Name;
        }
    }
}
