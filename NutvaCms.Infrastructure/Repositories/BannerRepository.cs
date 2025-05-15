using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class BannerRepository : IBannerRepository
{
    private readonly AppDbContext _db;

    public BannerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Banner>> GetAllAsync() =>
        await _db.Banners.Include(b => b.Images).ToListAsync();

    public async Task<Banner?> GetByIdAsync(Guid id) =>
        await _db.Banners.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Banner banner)
    {
        _db.Banners.Add(banner);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Banner banner)
    {
        _db.Banners.Update(banner);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Banner banner)
    {
        _db.Banners.Remove(banner);
        await _db.SaveChangesAsync();
    }
}
