using Chateq.Core.Domain.Models;

namespace Chateq.Core.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Chat> GetChatWithMessagesAsync(string chatName, int pageNumber, int pageSize);
}