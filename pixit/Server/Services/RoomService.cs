using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixit.Server.Models;

namespace pixit.Server.Services
{
    public class RoomService
    {
        private List<RoomModel> Rooms = new List<RoomModel>();

        public Task Create(RoomModel room)
        {
            return Task.Run(() => Rooms.Add(room));
        }

        public Task<List<RoomModel>> GetRooms()
        {
            return Task.Run(() => Rooms);
        }
    }
}
