using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using pixit.Shared.Models;

namespace pixit.Server.Services
{
    public class RoomService
    {
        private readonly ConcurrentDictionary<string, RoomModel> _rooms = new();

        public Task<KeyValuePair<string, RoomModel>> Create(RoomModel room)
        {
            string roomId = Guid.NewGuid().ToString();
            if (_rooms.TryAdd(roomId, room)) 
                return Task.FromResult(new KeyValuePair<string, RoomModel>(roomId, room));
            return Task.FromException<KeyValuePair<string, RoomModel>>(new Exception("Nie mozna dodac pokoju bo nie."));
        }

        public ConcurrentDictionary<string,RoomModel> GetRooms()
        {
            return _rooms;
        }
    }
}
