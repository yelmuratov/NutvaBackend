using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage?> GetByIdAsync(int id);
        Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId);
        Task AddAsync(ChatMessage message);
    }
}
