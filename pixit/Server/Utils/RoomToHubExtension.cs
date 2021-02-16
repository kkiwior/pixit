using Microsoft.AspNetCore.SignalR;
using pixit.Shared.Models;

namespace pixit.Server.Utils
{
    public class RoomToHubExtension : Hub
    {
        public RoomModel Room { get; set; }
    }
}