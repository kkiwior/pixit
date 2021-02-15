namespace pixit.Shared.Models.Events
{
    public class UserLeftRoomEvent
    {
        public string RoomId { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public string Host { get; set; }

        public UserLeftRoomEvent()
        {
        }

        public UserLeftRoomEvent(string userId, string Host = null)
        {
            Id = userId;
        }
    }
}