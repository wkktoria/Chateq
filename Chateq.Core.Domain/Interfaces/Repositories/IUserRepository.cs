using Chateq.Core.Domain.Models;

namespace Chateq.Core.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);

    Task<User?> GetUserByIdAsync(Guid id);

    Task<User?> GetUserByUsernameAsync(string username);
}