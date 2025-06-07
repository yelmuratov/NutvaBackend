using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _context;

        public BannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Banner>> GetAllAsync() =>
            await _context.Banners.ToListAsync();

        public async Task<Banner?> GetByIdAsync(Guid id) =>
            await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Banner banner)
        {
            var existing = await _context.Banners.FirstOrDefaultAsync(b => b.Id == banner.Id);
            if (existing is null)
                throw new InvalidOperationException($"Banner with ID {banner.Id} not found");

            // Fully update entire owned translations and other fields
            existing.En = banner.En;
            existing.Uz = banner.Uz;
            existing.Ru = banner.Ru;
            existing.Link = banner.Link;
            existing.ImageUrls = banner.ImageUrls;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }
    }
}
