using System.Collections.Generic;

namespace pixit.Shared.Models.Events
{
    public class RoomInfoEvent
    {
        public string Name { get; set; }
        public int Slots { get; set; }
        public int MaxScore { get; set; }
        public List<UserModel> Users { get; set; }
        public int CardsCount { get; set; }
        public bool Started { get; set; }
    }
}