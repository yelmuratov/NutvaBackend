using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace NutvaCms.Infrastructure.Repositories
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly AppDbContext _context;

        public ChatSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatSession> GetActiveSessionAsync(string anonymousId)
        {
            return await _context.ChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.AnonymousId == anonymousId && !s.IsClosed);
        }

        public async Task<ChatSession> CreateSessionAsync(string anonymousId, int adminId)
        {
            var session = new ChatSession
            {
                AnonymousId = anonymousId,
                AdminId = adminId
            };

            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task EndSessionAsync(Guid sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.IsClosed = true;
                session.EndedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

}
