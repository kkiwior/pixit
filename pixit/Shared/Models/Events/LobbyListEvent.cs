namespace pixit.Shared.Models.Events
{
    public class LobbyListEvent
    {
        public string Name { get; set; }
        public int Slots { get; set; }
        public int MaxScore { get; set; }
        public int UsersOnline { get; set; }
        public int CardsCount { get; set; }
    }
}