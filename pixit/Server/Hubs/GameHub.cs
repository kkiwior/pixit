using System.Threading.Tasks;
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
        
        public async Task VoteCard(string cardId)
        {
            await _games.VoteCard(Context.Items["room"]?.ToString(), Room, Context.ConnectionId, cardId);
        }

        public async Task Reconnect(ReconnectToGameEvent e)
        {
            await _games.Reconnect(e, Context.ConnectionId);
            Context.Items["room"] = e.RoomId;
        }
    }
}