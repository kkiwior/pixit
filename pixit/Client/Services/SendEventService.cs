using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using pixit.Shared.Models;

namespace pixit.Client.Services
{
    public class SendEventService
    {
        private readonly SignalRService _signalr;
        public SendEventService(SignalRService signalr)
        {
            _signalr = signalr;
        }
        
        public async Task CreateRoom(CreateRoomModel room) => await _signalr.HubConnection.SendAsync("CreateRoom", room);
    }
}