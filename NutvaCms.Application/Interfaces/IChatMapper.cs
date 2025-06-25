namespace NutvaCms.Application.Interfaces
{
    using NutvaCms.Application.DTOs.Chat;
    using NutvaCms.Domain.Entities;

    public interface IChatMapper
    {
        ChatMessageDto ToDto(ChatMessage message);
        ChatMessage ToEntity(ChatMessageDto dto, Guid sessionId);
    }

}