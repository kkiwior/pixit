using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace pixit.Shared.Models.Events
{
    public class JoinRoomEvent
    {
        public UserModel User { get; set; }
        public string RoomId { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public SettingsModel Settings { get; set; }
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public string HostId { get; set; }
        public bool Started { get; set; }

        public JoinRoomEvent()
        {
            Settings = new();
        }
    }
}