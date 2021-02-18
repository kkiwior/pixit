using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using pixit.Shared.Models.Others;

namespace pixit.Shared.Models
{
    public class GameModel : INotifyPropertyChanged
    {
        private GameState _state;
        private NarratorModel _narrator;
        private ObservableCollection<string> _waiting;
        private ObservableCollection<CardOnTableModel> _cardsOnTable;
        public event PropertyChangedEventHandler PropertyChanged;

        public int CardsOnTableCount { get; set; }
        public string Clue { get; set; }
        public List<ScoreEntryModel> Scoreboard { get; set; } = new();
        [JsonIgnore]
        public RNGModel RNG { get; set; }

        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }
        
        public NarratorModel Narrator
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
        
        public ObservableCollection<CardOnTableModel> CardsOnTable
        {
            get => _cardsOnTable;
            set
            {
                _cardsOnTable = value;
                _cardsOnTable.CollectionChanged += (_, _) => OnPropertyChanged("");
            }
        }
        
        public GameModel()
        {
            RNG = new();
            Waiting = new();
            Narrator = new();
            CardsOnTable = new();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}