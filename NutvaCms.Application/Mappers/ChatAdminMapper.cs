using NutvaCms.Domain.Entities;
using NutvaCms.Application.DTOs.Chat;

namespace NutvaCms.Application.Mappers
{
    public static class ChatAdminMapper
    {
        public static ChatAdminDto ToDto(ChatAdmin entity)
        {
            return new ChatAdminDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                TelegramUserId = entity.TelegramUserId,
                IsBusy = entity.IsBusy
            };
        }

        public static ChatAdmin ToEntity(CreateChatAdminDto dto)
        {
            return new ChatAdmin
            {
                FullName = dto.FullName,
                TelegramUserId = dto.TelegramUserId,
                IsBusy = false // Default new admins to not busy
            };
        }

        public static void UpdateEntity(ChatAdmin entity, UpdateChatAdminDto dto)
        {
            entity.FullName = dto.FullName;
            entity.TelegramUserId = dto.TelegramUserId;
            entity.IsBusy = dto.IsBusy;
        }
    }
}
