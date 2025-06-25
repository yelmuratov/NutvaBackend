using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.Application.Mappers
{
    public class ChatMapper : IChatMapper
    {
        public ChatMessageDto ToDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Sender = message.Sender,
                Message = message.Content,
                SentAt = message.SentAt
            };
        }

        public ChatMessage ToEntity(ChatMessageDto dto, Guid sessionId)
        {
            return new ChatMessage
            {
                ChatSessionId = sessionId,
                Sender = dto.Sender,
                Content = dto.Message,
                SentAt = dto.SentAt
            };
        }
    }
}
