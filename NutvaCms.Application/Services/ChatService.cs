using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using Telegram.Bot;

namespace NutvaCms.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatSessionRepository _sessionRepo;
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatAdminRepository _adminRepo;
        private readonly IChatMapper _mapper;
        private readonly ITelegramBotClient _bot;

        public ChatService(
            IChatSessionRepository sessionRepo,
            IChatMessageRepository messageRepo,
            IChatAdminRepository adminRepo,
            IChatMapper mapper,
            ITelegramBotClient bot)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
            _adminRepo = adminRepo;
            _mapper = mapper;
            _bot = bot;
        }

        public async Task SendMessageAsync(SendChatMessageDto dto, string anonymousId)
        {
            // 1. Try to get an active session for this user
            var session = await _sessionRepo.GetActiveSessionAsync(anonymousId);

            if (session == null)
            {
                // 2. Get available admin
                var admin = await _adminRepo.GetAvailableAdminAsync();
                if (admin == null)
                    throw new Exception("No admins available right now.");

                // 3. Create new session and mark admin as busy
                session = await _sessionRepo.CreateSessionAsync(anonymousId, admin.Id);
                admin.IsBusy = true;
                await _adminRepo.UpdateAsync(admin);

                // ⚠️ Ensure we have the TelegramUserId in session.Admin
                session.Admin = admin;
            }

            // 4. Send message to admin via Telegram
            await _bot.SendTextMessageAsync(
                chatId: session.Admin.TelegramUserId,
                text: $"💬 New message from anonymous user:\n\n{dto.Message}"
            );

            // 5. Save message to database
            var entity = new ChatMessage
            {
                ChatSessionId = session.Id,
                Sender = "user",
                Content = dto.Message
            };

            await _messageRepo.SaveMessageAsync(entity);
        }
    }
}
