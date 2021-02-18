using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace pixit.Shared.Models
{
    public class GameModel : INotifyPropertyChanged
    {
        private GameState _state;
        private Narrator _narrator;
        private ObservableCollection<string> _waiting;

        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }
        public Narrator Narrator
        {
            get => _narrator;
            set
            {
                _narrator = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Waiting
        {
            get => _waiting;
            set
            {
                _waiting = value;
                _waiting.CollectionChanged += (_, _) => OnPropertyChanged("");
            }
        }

        [JsonIgnore]
        public RNG RNG { get; set; }
        
        public string Clue { get; set; }
        
        [JsonIgnore]
        public List<CardOnTable> CardsOnTable { get; set; }

        public int CardsOnTableCount => CardsOnTable.Count;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameModel()
        {
            RNG = new();
            State = GameState.NarratorPicking;
            Waiting = new();
            Narrator = new();
            CardsOnTable = new();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CardOnTable
    {
        public CardModel Card { get; set; }
        public string UserId { get; set; }
        public bool IsNarratorCard { get; set; }
    }

    public class Narrator
    {
        public int Index { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public string Token { get; set; }

        public Narrator()
        {
            Index = -1;
        }
    }
    

    public class RNG
    {
        public int Seed { get; set; }
        public int Increment { get; set; }
        public int Max { get; set; }
    }
    
    public enum GameState
    {
        NarratorPicking = 0,
        UsersPicking = 1,
        UsersVoting = 2
    }
}