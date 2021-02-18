namespace pixit.Shared.Models.Events
{
    public class LobbyListEvent
    {
        public string Name { get; set; }
        public int UsersOnline { get; set; }
        public SettingsModel Settings { get; set; }
    }
}