using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace pixit.Shared.Models
{
    public class GameModel : INotifyPropertyChanged
    {
        private GameState _state;
        private Narrator _narrator;
        private ObservableCollection<string> _waiting;
        private ObservableCollection<CardOnTable> _cardsOnTable;

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
        
        public ObservableCollection<CardOnTable> CardsOnTable
        {
            get => _cardsOnTable;
            set
            {
                _cardsOnTable = value;
                _cardsOnTable.CollectionChanged += (_, _) => OnPropertyChanged("");
            }
        }

        [JsonIgnore]
        public RNG RNG { get; set; }
        
        public string Clue { get; set; }
        
        public List<ScoreEntry> Scoreboard { get; set; } = new();

        


        public int CardsOnTableCount { get; set; }

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
        public string Id { get; set; }
        public CardModel Card { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        [JsonIgnore]
        public bool IsNarratorCard { get; set; }

        public List<string> Votes { get; set; } = new();
    }

    public class ScoreEntry
    {
        public string UserId { get; set; }
        public int Score { get; set; }
    }

    public class Narrator
    {
        public int Index { get; set; }
        public string UserId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
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