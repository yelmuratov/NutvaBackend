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

    public async Task UpdateAsync(BlogPost updatedPost)
    {
        var existing = await _context.BlogPosts
            .Include(b => b.Media)
            .Include(b => b.En)
            .Include(b => b.Uz)
            .Include(b => b.Ru)
            .FirstOrDefaultAsync(b => b.Id == updatedPost.Id);

        if (existing == null)
            throw new Exception("Blog post not found");

        existing.Published = updatedPost.Published;

        // ✅ Append new media instead of clearing
        if (updatedPost.Media?.Any() == true)
        {
            var existingUrls = existing.Media.Select(m => m.Url).ToHashSet(); // optional dedup
            foreach (var media in updatedPost.Media)
            {
                if (!string.IsNullOrWhiteSpace(media.Url) && !existingUrls.Contains(media.Url))
                {
                    existing.Media.Add(new BlogPostMedia
                    {
                        Id = Guid.NewGuid(),
                        BlogPostId = existing.Id,
                        Url = media.Url,
                        MediaType = media.MediaType
                    });
                }
            }
        }

        existing.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existing).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BlogPost blogPost)
    {
        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        var blog = await _context.BlogPosts.FindAsync(id);
        if (blog != null)
        {
            blog.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}