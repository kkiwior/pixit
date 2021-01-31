using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pixit.Shared.Models.Events
{
    public class JoinRoomEvent
    {
        public UserModel User { get; set; }
        
        public string RoomId { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public int Slots { get; set; }
        public int MaxScore { get; set; }
        public List<UserModel> Users { get; set; }
        public int CardsCount { get; set; }
        public bool Started { get; set; }
    }
}