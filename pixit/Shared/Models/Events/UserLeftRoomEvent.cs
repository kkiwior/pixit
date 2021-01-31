namespace pixit.Shared.Models.Events
{
    public class UserLeftRoomEvent
    {
        public string RoomId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}