using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Hubs;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace pixit.Server.Repositiories
{
    public class RoomRepository
    {
        private readonly IRedisDatabase _rooms;

        public RoomRepository(IRedisCacheClient redisCache)
        {
            _rooms = redisCache.GetDbFromConfiguration();
            _rooms.FlushDbAsync();
        }

        public async Task<string> Create(RoomModel room)
        {
            string roomId = Guid.NewGuid().ToString();
            await _rooms.AddAsync(roomId, room);
            return roomId;
        }
        
        public async Task<IDictionary<string,LobbyListEvent>> GetAllRooms()
        {
            IEnumerable<string> keys = await _rooms.SearchKeysAsync("*");
            return await _rooms.GetAllAsync<LobbyListEvent>(keys);
        }
        
        public async Task<RoomModel> GetRoomById(string roomId)
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
    }
}