using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Client.Pages;
using pixit.Server.Hubs;
using pixit.Server.Repositiories;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

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
            room.Game.RNG.Increment = rnd.Next(1, 200);
            room.Game.RNG.Seed = rnd.Next(1, 1500);
            room.Game.RNG.Max = 1500;
            await ChangeNarrator(room);
            await RefillCardsForEveryone(roomId, room);
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task ChangeNarrator(RoomModel room)
        {
            room.Game.Narrator.Index++;
            if (room.UsersOnline < room.Game.Narrator.Index) room.Game.Narrator.Index = 0;
            room.Game.Narrator.Token = await Task.FromResult(room.Users[room.Game.Narrator.Index].Token);
            room.Game.Narrator.UserId = await Task.FromResult(room.Users[room.Game.Narrator.Index].Id);
            room.Game.Waiting.Clear();
            room.Game.Waiting.Add(room.Game.Narrator.UserId);
        }

        private async Task RefillCardsForEveryone(string roomId, RoomModel room)
        {
            List<Task> tasks = new();
            room.Users.ToList().ForEach(async u =>
            {
                while (u.CardDeck.Count < 6)
                {
                     tasks.Add(RefillUserCard(u, room.Game.RNG));
                }
            });
            await Task.WhenAll(tasks);
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

        public async Task SelectCard(string roomId, RoomModel room, string connectionid, SelectCardEvent cardEvent)
        {
            UserModel user = await Task.FromResult(room.Users.FirstOrDefault(u => u.Token == connectionid));
            if (user == null || !room.Game.Waiting.Contains(user.Id) || user.CardDeck.FirstOrDefault(c=>c.Id == cardEvent.Card.Id) == null) return;
            CardOnTable card = new CardOnTable()
            {
                Card = cardEvent.Card,
                UserId = user.Id,
                IsNarratorCard = false
            };
            
            if (room.Game.State == GameState.NarratorPicking && connectionid == room.Game.Narrator.Token)
            {
                room.Game.Clue = cardEvent.Clue;
                card.IsNarratorCard = true;
            }

            room.Game.Waiting.Remove(user.Id);
            room.Game.CardsOnTable.Add(card);
            user.CardDeck.Remove(cardEvent.Card);
            await RefillUserCard(user, room.Game.RNG);
            await _rooms.Save(roomId, room);
            
            if (room.Game.Waiting.Count == 0)
            {
                if (room.Game.State == GameState.NarratorPicking) await UsersPickingState(roomId, room, user.Id);
                //if (room.Game.State == GameState.UsersVoting)
            }

            await _hub.Clients.Client(connectionid).SendAsync("TakeCard", card.Card);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task UsersPickingState(string roomId, RoomModel room, string userId)
        {
            List<string> waiting = room.Users.ToList().Select(u => u.Id).ToList();
            waiting.Remove(userId);
            room.Game.Waiting.Clear();
            waiting.ForEach(u => room.Game.Waiting.Add(u));
            room.Game.State = GameState.UsersPicking;
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }
    }
}