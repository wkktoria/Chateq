using Chateq.Core.Domain;
using Chateq.Core.Domain.Exceptions;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chateq.Infrastructure.Repositories;

public class ChatRepository(ChateqDbContext context, ILogger<ChatRepository> logger) : IChatRepository
{
    public async Task<Chat> GetChatWithMessagesAsync(string chatName, int pageNumber, int pageSize)
    {
        try
        {
            var chat = await GetChatAsync(chatName, pageNumber, pageSize);

            if (chat != null)
            {
                return chat;
            }

            throw new ChatNotFoundException(chatName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while fetching chat with name: {chatName}");
            throw;
        }
    }

    private async Task<Chat?> GetChatAsync(string chatName, int pageNumber, int pageSize)
    {
        return await context.Chats
            .Where(c => c.Name == chatName)
            .Include(c => c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
            ).ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync();
    }
}