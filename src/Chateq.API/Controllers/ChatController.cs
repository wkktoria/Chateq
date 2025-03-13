using Chateq.Core.Domain.Exceptions;
using Chateq.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chateq.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService chatService, ILogger<ChatController> logger) : Controller
{
    [HttpPost("GetPaginatedChat")]
    public async Task<IActionResult> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
    {
        try
        {
            var chat = await chatService.GetPaginatedChatAsync(chatName, pageNumber, pageSize);
            return Json(chat);
        }
        catch (ChatNotFoundException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred during fetching chat with name: {chatName}");
            return StatusCode(500, "An unexpected error occurred during fetching chat.");
        }
    }
}