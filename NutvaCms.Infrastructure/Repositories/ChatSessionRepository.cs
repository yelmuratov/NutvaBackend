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

        public async Task<ChatSession?> GetByIdAsync(int id)
        {
            return await _context.ChatSessions
                .Include(s => s.ChatAdmin)
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<ChatSession>> GetAllAsync()
        {
            return await _context.ChatSessions
                .Include(s => s.ChatAdmin)
                .Include(s => s.Messages)
                .ToListAsync();
        }

        public async Task AddAsync(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatSession session)
        {
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatSession>> GetActiveSessionsAsync()
        {
            return await _context.ChatSessions
                .Where(s => s.IsActive)
                .Include(s => s.ChatAdmin)
                .Include(s => s.Messages)
                .ToListAsync();
        }

        public async Task<ChatSession?> GetActiveSessionByUserAsync(string userIdentifier)
        {
            return await _context.ChatSessions
                .Include(s => s.Messages)
                .Include(s => s.ChatAdmin)
                .FirstOrDefaultAsync(s => s.UserIdentifier == userIdentifier && s.IsActive);
        }

        public async Task<ChatSession?> GetActiveSessionByAdminTelegramIdAsync(long telegramUserId)
        {
            return await _context.ChatSessions
                .Include(s => s.Messages)
                .Include(s => s.ChatAdmin)
                .FirstOrDefaultAsync(s =>
                    s.IsActive
                    && s.ChatAdmin != null
                    && s.ChatAdmin.TelegramUserId == telegramUserId);
        }
    }
}
