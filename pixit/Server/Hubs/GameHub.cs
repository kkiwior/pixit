using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Server.Utils;

namespace pixit.Server.Hubs
{
    partial class RoomHub
    {
        public async Task StartGame()
        {
            await _games.StartGame(Context.Items["room"]?.ToString(), Room, Context.ConnectionId);
        }
    }
}