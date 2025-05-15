using Microsoft.EntityFrameworkCore;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly AppDbContext _context;

    public BlogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Blog>> GetAllAsync() =>
        await _context.Blogs.ToListAsync();

    public async Task<Blog?> GetByIdAsync(Guid id) =>
        await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Blog blog)
    {
        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Blog blog)
    {
        _context.Blogs.Update(blog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Blog blog)
    {
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
    }

    public async Task IncrementViewAsync(Guid blogId)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            blog.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}
