using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace NutvaCms.Infrastructure.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;

        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            return await _context.ChatMessages
                .Include(m => m.ChatSession)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatSessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}
