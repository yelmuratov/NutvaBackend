using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatAdminRepository
    {
        Task<ChatAdmin?> GetByIdAsync(int id);
        Task<List<ChatAdmin>> GetAllAsync();
        Task AddAsync(ChatAdmin admin);
        Task UpdateAsync(ChatAdmin admin);
        Task DeleteAsync(ChatAdmin admin);
        Task<ChatAdmin?> GetByTelegramUserIdAsync(long telegramUserId);
        Task<ChatAdmin?> GetAvailableAdminAsync();
    }
}
