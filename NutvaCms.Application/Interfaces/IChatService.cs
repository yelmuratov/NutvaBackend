using NutvaCms.Application.DTOs.Chat;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatSessionDto> StartChatAsync(StartChatDto dto); // create session + first message
        Task<ChatSessionDto?> GetSessionAsync(int sessionId);
        Task<ChatSessionDto?> GetActiveSessionByUserAsync(string userIdentifier);
        Task<ChatMessageDto> SendMessageAsync(SendChatMessageDto dto); // send new message to session
        Task<List<ChatMessageDto>> GetSessionMessagesAsync(int sessionId);
        Task EndSessionAsync(int sessionId);
        Task<ChatSessionDto?> GetActiveSessionByAdminTelegramIdAsync(long telegramUserId);
    }
}
