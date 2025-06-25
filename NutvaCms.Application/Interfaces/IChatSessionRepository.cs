using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IChatSessionRepository
    {
        Task<ChatSession> GetActiveSessionAsync(string anonymousId);
        Task<ChatSession> CreateSessionAsync(string anonymousId, int adminId);
        Task EndSessionAsync(Guid sessionId);
    }
}
