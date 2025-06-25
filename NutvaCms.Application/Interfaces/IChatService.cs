using NutvaCms.Application.DTOs.Chat;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatService
    {
        Task SendMessageAsync(SendChatMessageDto dto, string anonymousId);
    }
}
