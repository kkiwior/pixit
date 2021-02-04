using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Client.Services
{
    public class SendEventService
    {
        private readonly SignalRService _signalr;
        public SendEventService(SignalRService signalr)
        {
            _signalr = signalr;
        }
        
        public async Task GetRooms() => await _signalr.HubConnection.SendAsync("GetRooms");
        public async Task CreateRoom(CreateRoomModel room) => await _signalr.HubConnection.SendAsync("CreateRoom", room);
        public async Task UserJoinRoom(JoinRoomEvent session) => await _signalr.HubConnection.SendAsync("UserJoinRoom", session);
        public async Task UserLeftRoom(UserLeftRoomEvent session) => await _signalr.HubConnection.SendAsync("UserLeftRoom", session);
        public async Task UpdateSettings(SettingsModel settings) => await _signalr.HubConnection.SendAsync("UpdateSettings", settings);
    }
}