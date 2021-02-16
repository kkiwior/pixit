﻿using pixit.Shared.Models;

namespace pixit.Client.Utils
{
    public class StateContainer
    {
        public RoomModel Room { get; set; }
        public GameModel Game { get; set; }
        public string UserId { get; set; }
        public string JoinRoomAfterLogin { get; set; }

        public StateContainer()
        {
            Game = new GameModel();
        }
    }
}