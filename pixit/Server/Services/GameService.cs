using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Client.Pages;
using pixit.Server.Hubs;
using pixit.Server.Repositiories;
using pixit.Shared.Models;

namespace pixit.Server.Services
{
    public class GameService
    {
        private readonly IHubContext<RoomHub> _hub;
        private readonly RoomRepository _rooms;
        
        public GameService(RoomRepository rooms, IHubContext<RoomHub> hub)
        {
            _hub = hub;
            _rooms = rooms;
        }
        
        public async Task StartGame(string roomId, RoomModel room, string connectionid)
        {
            if (connectionid != room.Settings.HostToken || room.UsersOnline < 3) return;
            room.Started = true;
            Random rnd = new();
            room.Game.RNG.Increment = rnd.Next(1, 15);
            room.Game.RNG.Seed = rnd.Next(1, 38);
            room.Game.RNG.Max = 38;
            await RefillCardsForEveryone(roomId, room);
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task RefillCardsForEveryone(string roomId, RoomModel room)
        {
            room.Users.ToList().ForEach(async u =>
            {
                while (u.CardDeck.Count < 6)
                {
                    await RefillUserCard(u, room.Game.RNG);
                }
            });
        }

        private async Task RefillUserCard(UserModel user, RNG rng)
        {
            var card = await Task.FromResult(new CardModel()
            {
                Id = (rng.Seed + rng.Increment) % rng.Max
            });
            rng.Seed = card.Id;
            user.CardDeck.Add(card);
            await _hub.Clients.Client(user.Token).SendAsync("RefillCards", card);
        }
    }
}