using Chateq.Core.Domain.DTOs;

namespace Chateq.Core.Domain.Interfaces.Services;

public interface IChatService
{
    Task<ChatDto> GetPaginatedChatAsync(string chatName, int pageNumber, int pageSize);

    Task SaveMessageAsync(MessageDto messageDto);
}