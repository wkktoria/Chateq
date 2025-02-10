using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chateq.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepository) : Controller
{
    [HttpGet]
    public async Task<JsonResult> GetUser()
    {
        var user = new User()
        {
            Username = "user@1",
            Password = "password@1",
        };

        await userRepository.AddUserAsync(user);
        return Json(user);
    }
}