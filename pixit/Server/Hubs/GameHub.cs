using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Server.Utils;
using pixit.Shared.Models.Events;

namespace pixit.Server.Hubs
{
    partial class RoomHub
    {
        public async Task StartGame()
        {
            await _games.StartGame(Context.Items["room"]?.ToString(), Room, Context.ConnectionId);
        }

        public async Task SelectCard(SelectCardEvent cardEvent)
        {
            await _games.SelectCard(Context.Items["room"]?.ToString(), Room, Context.ConnectionId, cardEvent);
        }
    }
}