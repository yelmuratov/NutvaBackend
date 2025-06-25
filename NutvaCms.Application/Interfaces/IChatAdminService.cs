using NutvaCms.Application.DTOs.Chat;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatAdminService
    {
        Task<ChatAdminDto?> GetByIdAsync(int id);
        Task<List<ChatAdminDto>> GetAllAsync();
        Task<ChatAdminDto> CreateAsync(CreateChatAdminDto dto);
        Task<ChatAdminDto?> UpdateAsync(int id, UpdateChatAdminDto dto);
        Task<bool> DeleteAsync(int id);

        // Optionally, for advanced scenarios
        Task<ChatAdminDto?> GetByTelegramUserIdAsync(long telegramUserId);
        Task<ChatAdminDto?> GetAvailableAdminAsync();
    }
}
