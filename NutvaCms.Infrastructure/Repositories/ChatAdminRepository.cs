using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts; 
using Microsoft.EntityFrameworkCore;

namespace NutvaCms.Infrastructure.Repositories
{
    public class ChatAdminRepository : IChatAdminRepository
    {
        private readonly AppDbContext _context;

        public ChatAdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatAdmin?> GetByIdAsync(int id)
        {
            return await _context.ChatAdmins.FindAsync(id);
        }

        public async Task<List<ChatAdmin>> GetAllAsync()
        {
            return await _context.ChatAdmins.ToListAsync();
        }

        public async Task AddAsync(ChatAdmin admin)
        {
            await _context.ChatAdmins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatAdmin admin)
        {
            _context.ChatAdmins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ChatAdmin admin)
        {
            _context.ChatAdmins.Remove(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatAdmin?> GetByTelegramUserIdAsync(long telegramUserId)
        {
            return await _context.ChatAdmins
                .FirstOrDefaultAsync(a => a.TelegramUserId == telegramUserId);
        }

        public async Task<ChatAdmin?> GetAvailableAdminAsync()
        {
            return await _context.ChatAdmins
                .Where(a => a.IsOnline && !a.IsBusy)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();
        }
    }
}
