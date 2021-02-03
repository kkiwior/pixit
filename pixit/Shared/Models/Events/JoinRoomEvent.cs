using System.Collections.Generic;

namespace pixit.Shared.Models.Events
{
    public class JoinRoomEvent
    {
        public UserModel User { get; set; }
        public string RoomId { get; set; }
        public string Token { get; set; }
        
        
        
        public string Name { get; set; }
        public SettingsModel Settings { get; set; }
        public List<UserModel> Users { get; set; } = new();
        public bool Started { get; set; }

        public JoinRoomEvent()
        {
            Settings = new();
        }
    }
}