using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace NutvaCms.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public PublicChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // POST: api/PublicChat/send-message
        [HttpPost("send-message")]
        [AllowAnonymous] // allow users who are not logged in
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageDto dto)
        {
            // Validate incoming message
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Message cannot be empty.");

            // Try to get anonymous user ID from headers (e.g. X-Anonymous-Id), else generate a new one
            var anonymousId = Request.Headers["X-Anonymous-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

            try
            {
                // Call the service to send the message and route it to an available admin
                await _chatService.SendMessageAsync(dto, anonymousId);

                return Ok(new
                {
                    success = true,
                    message = "Message has been sent to an available admin."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
