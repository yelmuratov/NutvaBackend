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

    public async Task SaveMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ChatMessage>> GetMessagesBySessionAsync(Guid sessionId)
    {
        return await _context.ChatMessages
            .Where(m => m.ChatSessionId == sessionId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}

}
