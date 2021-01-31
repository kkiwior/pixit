namespace pixit.Shared.Models.Events
{
    public class JoinRoomEvent
    {
        public UserModel User { get; set; }
        public string RoomId { get; set; }
        public string Token { get; set; }
    }
}