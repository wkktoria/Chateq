using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chateq.API.Hubs;

[Authorize]
public class MessageHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}