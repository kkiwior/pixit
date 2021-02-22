namespace pixit.Shared.Models.Events
{
    public class ReconnectToGameEvent
    {
        public string Token { get; set; }
        public string RoomId { get; set; }
    }
}