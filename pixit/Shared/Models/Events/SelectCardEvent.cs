namespace pixit.Shared.Models.Events
{
    public class SelectCardEvent
    {
        public CardModel Card { get; set; }
        public string Clue { get; set; }
    }
}