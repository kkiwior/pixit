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

        public Task<KeyValuePair<string, RoomModel>> Create(CreateRoomModel roomData)
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
            if (_rooms.TryAdd(roomId, room)) 
                return Task.FromResult(new KeyValuePair<string, RoomModel>(roomId, room));
            return Task.FromException<KeyValuePair<string, RoomModel>>(new Exception("Nie mozna dodac pokoju bo nie."));
        }

        public ConcurrentDictionary<string,RoomModel> GetRooms()
        {
            return _rooms;
        }

        public Task<RoomModel> Get(string roomId)
        {
            RoomModel room;
            _rooms.TryGetValue(roomId, out room);
            return Task.FromResult(room);
        }
        
        public Task<RoomModel> GetInfo(string roomId, string Token)
        {
            RoomModel room;
            _rooms.TryGetValue(roomId, out room);
            if (room == null || room?.Users?.Exists(u=>u.Token == Token) == null) return null;
            return Task.FromResult(room);
        }
        
        public async Task<string> JoinRoom(string roomId, UserModel User)
        {
            RoomModel room = await this.Get(roomId);
            User.Token = Guid.NewGuid().ToString();
            room.Users.Add(User);
            return await Task.FromResult(User.Token);
        }
    }
}
