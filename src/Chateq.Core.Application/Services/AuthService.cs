using System.Security.Cryptography;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;

namespace Chateq.Core.Application.Services;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
    : IAuthService
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
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, $"Error occurred while registering user with username: {registerUser.Username}");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while registering user with username: {registerUser.Username}");
            throw new InvalidProgramException();
        }
    }

    public async Task<AuthDto> GetTokenAsync(LoginDto loginModel)
    {
        try
        {
            var user = await userRepository.GetUserByUsernameAsync(loginModel.Username);

            if (user == null)
            {
                logger.LogWarning($"User with username '{loginModel.Username}' does not exist.");
                throw new InvalidOperationException("User with this username does not exist.");
            }

            if (!VerifyPassword(loginModel.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var authData = jwtService.GenerateJwtToken(user);
            return authData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Authentication failed for user with username: {loginModel.Username}");
            throw;
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

    private static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var parts = storedPassword.Split(':');

        if (parts.Length != 2)
        {
            throw new FormatException(
                "Unexpected hash format. The stored hash should be in format: salt:hashedPassword");
        }

        var salt = Convert.FromBase64String(parts[0]);
        var storedHashedPassword = parts[1];
        var enteredHashedPassword = Hash(enteredPassword, salt);

        return enteredHashedPassword == storedHashedPassword;
    }
}