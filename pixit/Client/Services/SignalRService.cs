using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.IgnoreNullValues = true;
                })
                .Build();

            HubConnection.StartAsync();
            RegisterActions();
        }
        
        private void RegisterActions()
        {
            HubConnection.On<Dictionary<string, LobbyListEvent>>("SendRooms", rooms => _mediator.Notify(rooms));
            HubConnection.On<KeyValuePair<string, LobbyListEvent>>("SendRoom", rooms => _mediator.Notify(rooms));
            HubConnection.On<JoinRoomEvent>("JoinRoomEvent", rooms => _mediator.Notify(rooms));
            HubConnection.On<UserJoinedRoomEvent>("UserJoinedRoom", rooms => _mediator.Notify(rooms));
            HubConnection.On<UserLeftRoomEvent>("UserLeftRoom", rooms => _mediator.Notify(rooms));
        }
    }
}