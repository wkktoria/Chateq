using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chateq.API.Hubs;

[Authorize]
public class MessageHub(IUserConnectionService userConnectionService) : Hub
{
    public override Task OnConnectedAsync()
    {
        userConnectionService.AddConnection(Username, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        userConnectionService.RemoveConnection(Username);
        return base.OnDisconnectedAsync(exception);
    }

    private string Username => userConnectionService.GetClaimValue(Context.User, ClaimTypes.NameIdentifier);

    private string UserId => userConnectionService.GetClaimValue(Context.User, JwtRegisteredClaimNames.Jti);
}