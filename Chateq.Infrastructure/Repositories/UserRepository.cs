using Chateq.Core.Domain;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chateq.Infrastructure.Repositories;

public class UserRepository(ChateqDbContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task AddUserAsync(User user)
    {
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while adding user with Username: {user.Username}");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                logger.LogWarning($"Could not find user with Id: {id}");
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while fetching user with Id: {id}");
            throw;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                logger.LogWarning($"Could not find user with Username: {username}");
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while fetching user with Username: {username}");
            throw;
        }
    }
}