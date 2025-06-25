using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services
{
    public class ChatAdminService : IChatAdminService
    {
        private readonly IChatAdminRepository _repository;

        public ChatAdminService(IChatAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChatAdminDto?> GetByIdAsync(int id)
        {
            var admin = await _repository.GetByIdAsync(id);
            return admin == null ? null : ChatAdminMapper.ToDto(admin);
        }

        public async Task<List<ChatAdminDto>> GetAllAsync()
        {
            var admins = await _repository.GetAllAsync();
            return admins.Select(ChatAdminMapper.ToDto).ToList();
        }

        public async Task<ChatAdminDto> CreateAsync(CreateChatAdminDto dto)
        {
            var entity = ChatAdminMapper.ToEntity(dto);
            await _repository.AddAsync(entity);
            // After saving, entity.Id will be populated (if using EF Core)
            return ChatAdminMapper.ToDto(entity);
        }

        public async Task<ChatAdminDto?> UpdateAsync(int id, UpdateChatAdminDto dto)
        {
            var admin = await _repository.GetByIdAsync(id);
            if (admin == null) return null;

            ChatAdminMapper.UpdateEntity(admin, dto);
            await _repository.UpdateAsync(admin);

            return ChatAdminMapper.ToDto(admin);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admin = await _repository.GetByIdAsync(id);
            if (admin == null) return false;

            await _repository.DeleteAsync(admin);
            return true;
        }

        public async Task<ChatAdminDto?> GetByTelegramUserIdAsync(long telegramUserId)
        {
            var admin = await _repository.GetByTelegramUserIdAsync(telegramUserId);
            return admin == null ? null : ChatAdminMapper.ToDto(admin);
        }

        public async Task<ChatAdminDto?> GetAvailableAdminAsync()
        {
            var admin = await _repository.GetAvailableAdminAsync();
            return admin == null ? null : ChatAdminMapper.ToDto(admin);
        }
    }
}
