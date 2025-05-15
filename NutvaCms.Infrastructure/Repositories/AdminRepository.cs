using Microsoft.EntityFrameworkCore;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _db;

    public AdminRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Admin?> GetByEmailAsync(string email) =>
        await _db.Admins.FirstOrDefaultAsync(a => a.Email == email);

    public async Task<Admin?> GetByIdAsync(Guid id) =>
        await _db.Admins.FindAsync(id);

    public async Task<IEnumerable<Admin>> GetAllAsync() =>
        await _db.Admins.ToListAsync();

    public async Task AddAsync(Admin admin)
    {
        _db.Admins.Add(admin);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Admin admin)
    {
        _db.Admins.Update(admin);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Admin admin)
    {
        _db.Admins.Remove(admin);
        await _db.SaveChangesAsync();
    }
}
