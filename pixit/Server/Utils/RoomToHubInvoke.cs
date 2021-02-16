using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using pixit.Server.Repositiories;

namespace pixit.Server.Utils
{
    public class RoomToHubInvoke : IHubFilter
    {
        private readonly RoomRepository _rooms;
        
        public RoomToHubInvoke(RoomRepository rooms)
        {
            _rooms = rooms;
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            invocationContext.Hub.Context.Items.TryGetValue("room", out var roomId);


            if (roomId != null)
            {
                if (invocationContext.Hub is RoomToHubExtension toHub) toHub.Room = await _rooms.GetRoomById(roomId.ToString()); 
            }
            
            return await next(invocationContext);
        }
    }
}