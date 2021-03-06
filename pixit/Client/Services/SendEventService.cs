﻿using System.Threading.Tasks;
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
        public async Task CreateRoom(CreateRoomEvent room) => await _signalr.HubConnection.SendAsync("CreateRoom", room);
        public async Task UserJoinRoom(JoinRoomEvent session) => await _signalr.HubConnection.SendAsync("UserJoinRoom", session);
        public async Task UserLeftRoom() => await _signalr.HubConnection.SendAsync("UserLeftRoom");
        public async Task UpdateSettings(SettingsModel settings) => await _signalr.HubConnection.SendAsync("UpdateSettings", settings);
        public async Task KickUser(KickUserEvent user) => await _signalr.HubConnection.SendAsync("KickUser", user);
        public async Task StartGame() => await _signalr.HubConnection.SendAsync("StartGame");
        public async Task SelectCard(SelectCardEvent card) => await _signalr.HubConnection.SendAsync("SelectCard", card);
        public async Task VoteCard(string cardId) => await _signalr.HubConnection.SendAsync("VoteCard", cardId);
        public async Task Reconnect(ReconnectToGameEvent args) => await _signalr.HubConnection.SendAsync("Reconnect", args);
    }
}