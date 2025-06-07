using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly AppDbContext _context;

    public BlogPostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlogPost>> GetAllAsync()
    {
        return await _context.BlogPosts
            .Include(b => b.Media)
            .ToListAsync();
    }
    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await _context.BlogPosts
            .Include(b => b.Media)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(BlogPost blogPost)
    {
        await _context.BlogPosts.AddAsync(blogPost);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BlogPost blogPost)
    {
        _context.BlogPosts.Update(blogPost);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BlogPost blogPost)
    {
        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
    }
}
