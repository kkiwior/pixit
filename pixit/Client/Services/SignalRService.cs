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
            HubConnection.On<Dictionary<string, LobbyListEvent>>("SendRooms", args => _mediator.Notify(args));
            HubConnection.On<KeyValuePair<string, LobbyListEvent>>("SendRoom", args => _mediator.Notify(args));
            HubConnection.On<JoinRoomEvent>("JoinRoomEvent", args => _mediator.Notify(args));
            HubConnection.On<UserJoinedRoomEvent>("UserJoinedRoom", args => _mediator.Notify(args));
            HubConnection.On<UserLeftRoomEvent>("UserLeftRoom", args => _mediator.Notify(args));
            HubConnection.On<SettingsModel>("UpdateRoomSettings", args => _mediator.Notify(args));
            HubConnection.On<SetRoomHostEvent>("SetRoomHost", args => _mediator.Notify(args));
        }
    }
}