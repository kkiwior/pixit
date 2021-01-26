namespace pixit.Shared.Models
{
    public class RoomModel : CreateRoomModel
    {
        public int Id { get; set; }
        public int UsersOnline { get; set; }
        public int CardsCount { get; set; }
        public bool Started { get; set; }
    }
}
