using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using NutvaCms.Application.Services;
using NutvaCms.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using NutvaCms.API.Hubs;
using NutvaCms.Application.DTOs.Chat;

namespace NutvaCms.API.Controllers
{
    [ApiController]
    [Route("api/telegram/webhook")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public TelegramWebhookController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update.Message == null || update.Message.From == null)
                return Ok();

            // Get admin by TelegramUserId
            long adminTelegramId = update.Message.From.Id;
            string text = update.Message.Text ?? "";

            // Find active session assigned to this admin
            var session = await _chatService.GetActiveSessionByAdminTelegramIdAsync(adminTelegramId);
            if (session != null)
            {
                // Save admin reply as chat message
                await _chatService.SendMessageAsync(new SendChatMessageDto
                {
                    SessionId = session.Id,
                    Text = text,
                    Sender = "admin"
                });

                // Push to website user in real time
                await _hubContext.Clients.Group(session.Id.ToString())
                    .SendAsync("ReceiveMessage", new
                    {
                        Sender = "admin",
                        Text = text,
                        SentAt = DateTime.UtcNow
                    });
            }

            return Ok();
        }
    }
}
