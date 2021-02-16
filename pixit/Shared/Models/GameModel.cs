namespace pixit.Shared.Models
{
    public class GameModel
    {

        public GameState State { get; set; }

        public GameModel()
        {
            State = GameState.UsersPicking;
        }
    }
    
    public enum GameState
    {
        NarratorPicking = 0,
        UsersPicking = 1,
        UsersVoting = 2
    }
}