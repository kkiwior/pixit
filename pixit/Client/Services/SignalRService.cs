using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using pixit.Client.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

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
                .WithUrl(client.BaseAddress?.AbsoluteUri + "roomhub")
                .Build();

            HubConnection.StartAsync();
            RegisterActions();
        }
        
        private void RegisterActions()
        {
            HubConnection.On<Dictionary<string, RoomModel>>("SendRooms", rooms => _mediator.Notify(rooms));
            HubConnection.On<KeyValuePair<string, RoomModel>>("SendRoom", rooms => _mediator.Notify(rooms));
            HubConnection.On<JoinRoomEvent>("JoinRoomEvent", rooms => _mediator.Notify(rooms));
            HubConnection.On<RoomModel>("RoomInfo", rooms => _mediator.Notify(rooms));
            HubConnection.On<UserModel>("UserJoinedRoom", rooms => _mediator.Notify(rooms));
        }
    }
}