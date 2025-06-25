using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatMessageRepository
    {
        Task SaveMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesBySessionAsync(Guid sessionId);
    }
}
