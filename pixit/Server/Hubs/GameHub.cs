using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Services;
using pixit.Server.Utils;

namespace pixit.Server.Hubs
{
    public class GameHub : RoomToHubExtension
    {
        private readonly GameService _games;

        public GameHub(GameService gameService)
        {
            _games = gameService;
        }

        protected GameHub()
        {
        }

        public async Task StartGame()
        {
            
        }
    }
}