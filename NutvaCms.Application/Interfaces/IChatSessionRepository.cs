using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatSessionRepository
    {
        Task<ChatSession?> GetByIdAsync(int id);
        Task<List<ChatSession>> GetAllAsync();
        Task AddAsync(ChatSession session);
        Task UpdateAsync(ChatSession session);
        Task<List<ChatSession>> GetActiveSessionsAsync();
        Task<ChatSession?> GetActiveSessionByUserAsync(string userIdentifier);
        Task<ChatSession?> GetActiveSessionByAdminTelegramIdAsync(long telegramUserId);
    }
}
