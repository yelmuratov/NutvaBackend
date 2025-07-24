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

        // ✅ Only update Published status (don't touch translations if they're empty)
        existing.Published = updatedPost.Published;

        // ✅ DON'T update translations if they're empty - keep existing values
        // CopyTranslation methods are removed for media-only updates

        // ✅ Handle media collection - Clear existing media first
        if (existing.Media.Any())
        {
            existing.Media.Clear();
        }

        // ✅ Add new media
        if (updatedPost.Media?.Any() == true)
        {
            foreach (var media in updatedPost.Media)
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

        // ✅ CRITICAL: Force EF to save by updating a timestamp or touching the entity
        existing.ViewCount = existing.ViewCount; // This forces EF to detect change
        
        // ✅ Mark as modified
        _context.Entry(existing).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    private void CopyTranslation(BlogPostTranslation existing, BlogPostTranslation updated)
    {
        if (updated == null || existing == null) return;

        // Only update if the new value is not null or empty
        if (!string.IsNullOrEmpty(updated.Title))
            existing.Title = updated.Title;
        
        if (!string.IsNullOrEmpty(updated.Subtitle))
            existing.Subtitle = updated.Subtitle;
        
        if (!string.IsNullOrEmpty(updated.Content))
            existing.Content = updated.Content;
        
        if (!string.IsNullOrEmpty(updated.MetaTitle))
            existing.MetaTitle = updated.MetaTitle;
        
        if (!string.IsNullOrEmpty(updated.MetaDescription))
            existing.MetaDescription = updated.MetaDescription;
        
        if (!string.IsNullOrEmpty(updated.MetaKeywords))
            existing.MetaKeywords = updated.MetaKeywords;
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