using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly AppDbContext _db;

    public BlogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Blog>> GetAllAsync() =>
        await _db.Blogs.Include(b => b.Images).ToListAsync();

    public async Task<Blog?> GetByIdAsync(Guid id) =>
        await _db.Blogs.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Blog blog)
    {
        _db.Blogs.Add(blog);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Blog blog)
    {
        _db.Blogs.Update(blog);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Blog blog)
    {
        _db.Blogs.Remove(blog);
        await _db.SaveChangesAsync();
    }
}
