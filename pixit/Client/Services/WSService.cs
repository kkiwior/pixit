using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using pixit.Server.Models;

namespace pixit.Client.Services
{
    public class WSService
    {
        private readonly HttpClient _client;
        private HubConnection _hubConnection;

        public EventHandler<UserModel> UserJoined;
        public EventHandler<List<RoomModel>> GetRooms;

        public WSService(HttpClient client)
        {
            _client = client;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_client.BaseAddress.AbsoluteUri + "roomhub")
                .Build();

            RegisterActions();

            _hubConnection.StartAsync();

        }

        private void RegisterActions()
        {
            _hubConnection.On<UserModel>("UserJoined", (user) =>
            {
                UserJoined.Invoke(this, user);
            });

            _hubConnection.On<List<RoomModel>>("GetRooms", (rooms) =>
            {
                GetRooms.Invoke(this, rooms);
            });
        }

        public Task UserJoin(UserModel user) => _hubConnection.SendAsync("UserJoin", user);
        public Task CreateRoom(RoomModel room) => _hubConnection.SendAsync("CreateRoom", room);

    }
}