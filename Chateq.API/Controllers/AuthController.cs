using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chateq.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public async Task<JsonResult> GetUser()
    {
        await authService.RegisterUserAsync(new RegisterUserDto()
        {
            Username = "test",
            Password = "test",
        });

        return Json("");
    }
}