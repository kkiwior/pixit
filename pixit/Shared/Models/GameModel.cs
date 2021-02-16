using Newtonsoft.Json;

namespace pixit.Shared.Models
{
    public class GameModel
    {

        public GameState State { get; set; }
        
        [JsonIgnore]
        public RNG RNG { get; set; }

        public GameModel()
        {
            RNG = new();
            State = GameState.NarratorPicking;
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