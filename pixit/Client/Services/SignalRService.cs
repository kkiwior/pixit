﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Mapster;
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
        private StateContainer _state;
        private ILocalStorageService _localStorage;

        public SignalRService(HttpClient client, Mediator mediator, StateContainer state, ILocalStorageService localStorage)
        {
            _state = state;
            _mediator = mediator;
            _localStorage = localStorage;
            HubConnection = new HubConnectionBuilder()
                .WithUrl(client.BaseAddress?.AbsoluteUri + "roomhub")
                .AddJsonProtocol(options =>
                {
                    //options.PayloadSerializerOptions.IgnoreNullValues = true;
                    options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                })
                .Build();

            HubConnection.StartAsync();
            RegisterActions();
        }
        
        private void RegisterActions()
        {
            HubConnection.On<Dictionary<string, LobbyListEvent>>("SendRooms", args => _mediator.Notify(args));
            HubConnection.On<KeyValuePair<string, LobbyListEvent>>("SendRoom", args => _mediator.Notify(args));
            HubConnection.On<JoinRoomEvent>("JoinRoomEvent", args =>
            {
                _localStorage.SetItemAsync("token", args.Token);
                _localStorage.SetItemAsync("room", args.RoomId);
                _state.Room = new RoomModel(args.Name)
                {
                    Settings = args.Settings,
                    Users = args.Users,
                    Started = args.Started,
                    HostId = args.HostId
                };
                _state.UserId = args.UserId;
                return _mediator.Notify(args);
            });
            HubConnection.On<CreateRoomEvent>("CreateRoom", args => _mediator.Notify(args));
            HubConnection.On<KickUserEvent>("KickUser", args => _mediator.Notify(args));
            HubConnection.On<GameModel>("UpdateGameState", args =>
            {
                _state.Room.Game = args;
                _mediator.Notify(args);
            });
  
            HubConnection.On<SettingsModel>("UpdateRoomSettings", args => _state.Room.Settings = args);
            HubConnection.On<UserJoinedRoomEvent>("UserJoinedRoom", args => _state.Room.Users.Add(args.Adapt<UserModel>()));
            HubConnection.On<UserLeftRoomEvent>("UserLeftRoom", args => _state.Room.Users.Remove(_state.Room.Users.FirstOrDefault(u=>u.Id == args.Id)));
            HubConnection.On<SetRoomHostEvent>("SetRoomHost", args => _state.Room.HostId = args.HostId);
            HubConnection.On<CardModel>("RefillCards", args => _state.CardDeck.Add(args));
            HubConnection.On<CardModel>("TakeCard", args => _state.CardDeck.Remove(_state.CardDeck.FirstOrDefault(u=>u.Id == args.Id)));
        }
    }
}