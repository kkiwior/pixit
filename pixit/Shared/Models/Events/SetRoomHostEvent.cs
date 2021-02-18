namespace pixit.Shared.Models.Events
{
    public class SetRoomHostEvent
    {
        public string HostId { get; set; }

        public SetRoomHostEvent()
        {
        }
        
        public SetRoomHostEvent(string host)
        {
            HostId = host;
        }
    }
}