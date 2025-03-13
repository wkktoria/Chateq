using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chateq.API.Hubs;

[Authorize]
public class MessageHub(IUserConnectionService userConnectionService, IChatService chatService) : Hub
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

        var messageDto = CreateMessage(chatId, message);
        await chatService.SaveMessageAsync(messageDto);
    }

    private async Task JoinChatAsync(string chatName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
    }

    private async Task LeaveChatAsync(string chatName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
    }

    private MessageDto CreateMessage(string chatId, string message)
    {
        return new MessageDto
        {
            MessageText = message,
            ChatId = Guid.Parse(chatId),
            Sender = Username,
            SenderId = Guid.Parse(UserId)
        };
    }

    private string Username => userConnectionService.GetClaimValue(Context.User, ClaimTypes.NameIdentifier);

    private string UserId => userConnectionService.GetClaimValue(Context.User, JwtRegisteredClaimNames.Jti);

    private const string MainChat = "Global";
}