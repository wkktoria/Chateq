using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chateq.API.Hubs;

[Authorize]
public class MessageHub(IUserConnectionService userConnectionService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        userConnectionService.AddConnection(Username, Context.ConnectionId);
        await JoinChatAsync(MainChat);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        userConnectionService.RemoveConnection(Username);
        await LeaveChatAsync(MainChat);
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessageToChatAsync(string chatId, string message)
    {
        await Clients.Group(MainChat).SendAsync("ReceiveMessage", Username, message);
    }

    private async Task JoinChatAsync(string chatName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
    }

    private async Task LeaveChatAsync(string chatName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
    }

    private string Username => userConnectionService.GetClaimValue(Context.User, ClaimTypes.NameIdentifier);

    private string UserId => userConnectionService.GetClaimValue(Context.User, JwtRegisteredClaimNames.Jti);

    private const string MainChat = "Global";
}