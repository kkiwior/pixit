﻿using System;
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
            room.Users.ToList().ForEach(u => room.Game.Scoreboard.Add(new ScoreEntry()
            {
                UserId = u.Id,
                Score = 0
            }));
            await ChangeNarrator(room);
            await RefillCardsForEveryone(roomId, room);
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task ChangeNarrator(RoomModel room)
        {
            room.Game.State = GameState.NarratorPicking;
            room.Game.Narrator.Index++;
            if (room.UsersOnline == room.Game.Narrator.Index) room.Game.Narrator.Index = 0;
            room.Game.Narrator.Token = await Task.FromResult(room.Users[room.Game.Narrator.Index].Token);
            room.Game.Narrator.UserId = await Task.FromResult(room.Users[room.Game.Narrator.Index].Id);
            room.Game.Waiting.Clear();
            room.Game.Waiting.Add(room.Game.Narrator.UserId);
            room.Game.CardsOnTable.Clear();
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
                Id = Guid.NewGuid().ToString(),
                Card = cardEvent.Card,
                UserId = user.Id,
                IsNarratorCard = false
            };
            
            if (room.Game.State == GameState.NarratorPicking && connectionid == room.Game.Narrator.Token)
            {
                room.Game.CardsOnTable.Clear();
                room.Game.Clue = cardEvent.Clue;
                card.IsNarratorCard = true;
            }

            room.Game.Waiting.Remove(user.Id);
            room.Game.CardsOnTable.Add(card);
            room.Game.CardsOnTableCount = room.Game.CardsOnTable.Count;
            user.CardDeck.Remove(cardEvent.Card);
            await RefillUserCard(user, room.Game.RNG);

            if (room.Game.Waiting.Count == 0)
            {
                if (room.Game.State == GameState.UsersPicking) await ShowCards(roomId, room);
                if (room.Game.State == GameState.NarratorPicking) await UsersPickingState(roomId, room, user.Id);
            }
            
            await _rooms.Save(roomId, room);
            if(room.Game.State != GameState.UsersVoting) room.Game.CardsOnTable.Clear();
            await _hub.Clients.Client(connectionid).SendAsync("TakeCard", card.Card);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task ShowCards(string roomId, RoomModel room)
        {
            List<string> waiting = await Task.FromResult(room.Users.ToList().Select(u => u.Id).ToList());
            waiting.Remove(room.Game.Narrator.UserId);
            waiting.ForEach(u => room.Game.Waiting.Add(u));
            room.Game.State = GameState.UsersVoting;
            room.Game.CardsOnTable = new(room.Game.CardsOnTable.OrderBy(c => Guid.NewGuid()));
        }

        private async Task UsersPickingState(string roomId, RoomModel room, string userId)
        {
            List<string> waiting = room.Users.ToList().Select(u => u.Id).ToList();
            waiting.Remove(userId);
            waiting.ForEach(u => room.Game.Waiting.Add(u));
            room.Game.State = GameState.UsersPicking;
        }

        public async Task VoteCard(string roomId, RoomModel room, string connectionid, string cardId)
        {
            UserModel user = await Task.FromResult(room.Users.FirstOrDefault(u => u.Token == connectionid));
            if (user == null || !room.Game.Waiting.Contains(user.Id)) return;
            CardOnTable card = await Task.FromResult(room.Game.CardsOnTable.FirstOrDefault(c => c.Id == cardId));
            if (card == null || card.UserId == user.Id) return;
            card.Votes.Add(user.Id);
            room.Game.Waiting.Remove(user.Id);

            if (room.Game.Waiting.Count == 0)
            {
                await SetScore(roomId, room);
                await ChangeNarrator(room);
            }
            
            await _rooms.Save(roomId, room);
            await _hub.Clients.Group(roomId).SendAsync("UpdateGameState", room.Game);
        }

        private async Task SetScore(string roomId, RoomModel room)
        {
            CardOnTable narratorCard = await Task.FromResult(room.Game.CardsOnTable.FirstOrDefault(c=>c.IsNarratorCard));
            if (narratorCard.Votes.Count == room.UsersOnline - 1 || narratorCard.Votes.Count == 0)
            {
                room.Game.Scoreboard.ForEach(s =>
                {
                    if (s.UserId != narratorCard.UserId) s.Score += 2;
                });
                return;
            }
            List<CardOnTable> cards = await Task.FromResult(room.Game.CardsOnTable.ToList());
            cards.ForEach(c =>
            {
                if (c.IsNarratorCard)
                {
                    c.Votes.ForEach(v =>
                    {
                        room.Game.Scoreboard.Find(s => s.UserId == v).Score += 3;
                        room.Game.Scoreboard.Find(s => s.UserId == c.UserId).Score += 3;
                    });
                }
                else
                {
                    room.Game.Scoreboard.Find(s => s.UserId == c.UserId).Score += (1 * c.Votes.Count);
                }
            });
        }
    }
}