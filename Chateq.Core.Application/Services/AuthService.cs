using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Chateq.Core.Application.Services;

public class AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
{
    public async Task RegisterUserAsync(RegisterUserDto registerUser)
    {
        try
        {
            var existingUser = await userRepository.GetUserByUsernameAsync(registerUser.Username);

            if (existingUser != null)
            {
                logger.LogWarning($"User with username '{registerUser.Username}' already exists.");
                throw new InvalidOperationException("User with this username already exists.");
            }

            var user = new User(registerUser.Username, registerUser.Password);
            await userRepository.AddUserAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while registering user with username: {registerUser.Username}");
            throw new InvalidProgramException();
        }
    }
}