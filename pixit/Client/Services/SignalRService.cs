using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using pixit.Client.Shared;
using pixit.Shared.Models;

namespace pixit.Client.Services
{
    public class SignalRService
    {
        internal readonly HubConnection HubConnection;
        private readonly Mediator _mediator;

        public SignalRService(HttpClient client, Mediator mediator)
        {
            _mediator = mediator;
            HubConnection = new HubConnectionBuilder()
                .WithUrl(client.BaseAddress.AbsoluteUri + "roomhub")
                .Build();

            HubConnection.StartAsync();
            RegisterActions();
        }
        
        private void RegisterActions()
        {
            HubConnection.On<UserModel>("UserJoined", (user) => _mediator.Notify<UserModel>(user));
            HubConnection.On<List<RoomModel>>("GetRooms", (rooms) => _mediator.Notify<List<RoomModel>>(rooms));
        }
    }
}