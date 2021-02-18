using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using pixit.Shared.Models;

namespace pixit.Client.Utils
{
    public class StateContainer : INotifyPropertyChanged
    {
        private RoomModel _room;
        private ObservableCollection<CardModel> _cardDeck;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
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
        
        public ObservableCollection<CardModel> CardDeck
        {
            get => _cardDeck;
            set
            {
                _cardDeck = value;
                _cardDeck.CollectionChanged += (_, _) => OnPropertyChanged("");
            }
        }

        public StateContainer()
        {
            Room = new RoomModel(null);
            _cardDeck = new();
        }
        
        
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnNestedPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}