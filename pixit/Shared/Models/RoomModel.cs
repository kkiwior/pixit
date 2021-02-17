using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace pixit.Shared.Models
{
    public class RoomModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int UsersOnline => Users.Count;
        public SettingsModel Settings { get; set; }
        public bool Started { get; set; }
        private string _hostId;
        private GameModel _game;
        private ObservableCollection<UserModel> _users;
        
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set
            {
                _users = value;
                _users.CollectionChanged += (sender, args) => OnPropertyChanged("");
            }
        }

        public string HostId
        {
            get => _hostId;
            set
            {
                _hostId = value;
                OnPropertyChanged();
            }
        }

        public GameModel Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged();
            }
        }

        public RoomModel(string name)
        {
            Name = name;
            Settings = new();
            Game = new();
            Users = new();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
