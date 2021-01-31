namespace pixit.Shared.Models.Events
{
    public class UserJoinedRoomEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AvatarModel Avatar { get; set; } = new();
    }
}