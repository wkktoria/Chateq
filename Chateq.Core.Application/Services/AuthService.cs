using System.Security.Cryptography;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;

namespace Chateq.Core.Application.Services;

public class AuthService(IUserRepository userRepository, ILogger<AuthService> logger) : IAuthService
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

            var user = new User(registerUser.Username, HashPassword(registerUser.Password));
            await userRepository.AddUserAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while registering user with username: {registerUser.Username}");
            throw new InvalidProgramException();
        }
    }

    private static string HashPassword(string password)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hashed = Hash(password, salt);
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }

    private static string Hash(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256,
            10_000, 256 / 8)
        );
    }
}