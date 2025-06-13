using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using NutvaCms.API.Hubs;

namespace NutvaCms.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        // Start chat (session + first message)
        [HttpPost("start")]
        public async Task<IActionResult> StartChat([FromBody] StartChatDto dto)
        {
            var session = await _chatService.StartChatAsync(dto);

            // Push first message in real time (if any)
            var firstMessage = session.Messages.FirstOrDefault();
            if (firstMessage != null)
            {
                await _hubContext.Clients.Group(session.Id.ToString())
                    .SendAsync("ReceiveMessage", firstMessage);
            }

            return Ok(session);
        }

        // Send a new message to an existing session
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageDto dto)
        {
            var message = await _chatService.SendMessageAsync(dto);

            // Real-time push to all clients in the chat session
            await _hubContext.Clients.Group(dto.SessionId.ToString())
                .SendAsync("ReceiveMessage", message);

            return Ok(message);
        }

        // Get active session by user identifier
        [HttpGet("active/{userIdentifier}")]
        public async Task<IActionResult> GetActiveSession(string userIdentifier)
        {
            var session = await _chatService.GetActiveSessionByUserAsync(userIdentifier);
            if (session == null)
                return NotFound();
            return Ok(session);
        }

        // Get messages for a session
        [HttpGet("{sessionId}/messages")]
        public async Task<IActionResult> GetMessages(int sessionId)
        {
            var messages = await _chatService.GetSessionMessagesAsync(sessionId);
            return Ok(messages);
        }

        // End a session
        [HttpPost("{sessionId}/end")]
        public async Task<IActionResult> EndSession(int sessionId)
        {
            await _chatService.EndSessionAsync(sessionId);

            // Optionally: notify clients that chat ended
            await _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync("SessionEnded");

            return NoContent();
        }
    }
}
