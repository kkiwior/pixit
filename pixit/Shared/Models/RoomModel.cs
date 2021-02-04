using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pixit.Shared.Models
{
    public class RoomModel
    {
        public string Name { get; set; }
        public List<UserModel> Users { get; set; } = new();
        
        public SettingsModel Settings { get; set; }

        public int UsersOnline
        {
            get => Users.Count;
        }
        
        public bool Started { get; set; }

        public bool isHost { get; set; }

        public RoomModel(string name)
        {
            Name = name;
            Settings = new();
            isHost = false;
        }
    }
}
