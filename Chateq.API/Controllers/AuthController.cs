using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chateq.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : Controller
{
    [HttpPut("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
    {
        try
        {
            await authService.RegisterUserAsync(registerUser);
            return Ok(new { message = "User has been registered successfully." });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, $"Registration attempt failed: {ex.Message}");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                $"An error occurred during registration for user with username: {registerUser.Username}");
            return StatusCode(500, "An unexpected error occurred during registration.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
    {
        try
        {
            var authData = await authService.GetTokenAsync(loginModel);
            return Ok(authData);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred during login for user with username: {loginModel.Username}");
            return StatusCode(500, "An unexpected error occurred during login.");
        }
    }
}