using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using pixit.Shared.Models;

namespace pixit.Client.Utils
{
    public class StateContainer : INotifyPropertyChanged
    {
        private RoomModel _room;
        private GameModel _game;
        private List<CardModel> _cardDeck;
        public string UserId { get; set; }
        public string JoinRoomAfterLogin { get; set; }

        public RoomModel Room
        {
            get => _room;
            set
            {
                _room = value;
                _room.PropertyChanged += OnNestedPropertyChanged;
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
        public List<CardModel> CardDeck
        {
            get => _cardDeck;
            set
            {
                _cardDeck = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnNestedPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        public StateContainer()
        {
            Room = new RoomModel(null);
            _game = new GameModel();
            _cardDeck = new List<CardModel>();
        }
    }
}