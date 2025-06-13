using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Mappers
{
    public static class ChatMapper
    {
        // Session entity to DTO (with messages)
        public static ChatSessionDto ToDto(ChatSession session)
        {
            return new ChatSessionDto
            {
                Id = session.Id,
                ChatAdminId = session.ChatAdminId,
                ChatAdminName = session.ChatAdmin?.FullName,
                UserIdentifier = session.UserIdentifier,
                IsActive = session.IsActive,
                CreatedAt = session.CreatedAt,
                EndedAt = session.EndedAt,
                Messages = session.Messages?
                    .OrderBy(m => m.SentAt)
                    .Select(ToDto)
                    .ToList() ?? new()
            };
        }

        // Message entity to DTO
        public static ChatMessageDto ToDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                Sender = message.Sender,
                Text = message.Text,
                SentAt = message.SentAt
            };
        }

        // StartChatDto to new Session and Message entities
        public static (ChatSession, ChatMessage) FromStartChatDto(StartChatDto dto)
        {
            var session = new ChatSession
            {
                UserIdentifier = dto.UserIdentifier,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Messages = new List<ChatMessage>()
            };
            var message = new ChatMessage
            {
                Sender = "user", // always user for first message
                Text = dto.Text,
                SentAt = DateTime.UtcNow,
                ChatSession = session
            };
            session.Messages.Add(message);
            return (session, message);
        }

        // SendChatMessageDto to Message entity (for existing session)
        public static ChatMessage FromSendChatMessageDto(SendChatMessageDto dto)
        {
            return new ChatMessage
            {
                ChatSessionId = dto.SessionId,
                Sender = dto.Sender,
                Text = dto.Text,
                SentAt = DateTime.UtcNow
            };
        }
    }
}
