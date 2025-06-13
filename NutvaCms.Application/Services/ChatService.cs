using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;

namespace NutvaCms.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatSessionRepository _sessionRepo;
        private readonly IChatMessageRepository _messageRepo;

        public ChatService(
            IChatSessionRepository sessionRepo,
            IChatMessageRepository messageRepo)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
        }

        public async Task<ChatSessionDto> StartChatAsync(StartChatDto dto)
        {
            // Check if user already has an active session
            var existing = await _sessionRepo.GetActiveSessionByUserAsync(dto.UserIdentifier);
            if (existing != null)
                return ChatMapper.ToDto(existing);

            var (session, message) = ChatMapper.FromStartChatDto(dto);
            await _sessionRepo.AddAsync(session);
            // EF Core will cascade-save message because it's attached to session.Messages

            return ChatMapper.ToDto(session);
        }

        public async Task<ChatSessionDto?> GetSessionAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            return session == null ? null : ChatMapper.ToDto(session);
        }

        public async Task<ChatSessionDto?> GetActiveSessionByUserAsync(string userIdentifier)
        {
            var session = await _sessionRepo.GetActiveSessionByUserAsync(userIdentifier);
            return session == null ? null : ChatMapper.ToDto(session);
        }

        public async Task<ChatMessageDto> SendMessageAsync(SendChatMessageDto dto)
        {
            var message = ChatMapper.FromSendChatMessageDto(dto);
            await _messageRepo.AddAsync(message);
            return ChatMapper.ToDto(message);
        }

        public async Task<List<ChatMessageDto>> GetSessionMessagesAsync(int sessionId)
        {
            var messages = await _messageRepo.GetMessagesBySessionIdAsync(sessionId);
            return messages.Select(ChatMapper.ToDto).ToList();
        }

        public async Task EndSessionAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) return;

            session.IsActive = false;
            session.EndedAt = DateTime.UtcNow;
            await _sessionRepo.UpdateAsync(session);
        }

        public async Task<ChatSessionDto?> GetActiveSessionByAdminTelegramIdAsync(long telegramUserId)
        {
            var session = await _sessionRepo.GetActiveSessionByAdminTelegramIdAsync(telegramUserId);
            return session == null ? null : ChatMapper.ToDto(session);
        }
    }
}
