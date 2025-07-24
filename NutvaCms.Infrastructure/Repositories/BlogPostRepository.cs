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

        // ✅ FORCE a change to the main entity so EF detects modification
        // Option 1: If you have UpdatedAt property (recommended)
        // existing.UpdatedAt = DateTime.UtcNow;
        
        // Option 2: If you don't have UpdatedAt, force a change by setting properties to themselves
        existing.Published = updatedPost.Published;
        existing.ViewCount = existing.ViewCount; // This forces EF to detect change

        // ✅ Update scalar properties
        _context.Entry(existing).CurrentValues.SetValues(updatedPost);

        // ✅ Handle translations
        CopyTranslation(existing.En, updatedPost.En);
        CopyTranslation(existing.Uz, updatedPost.Uz);
        CopyTranslation(existing.Ru, updatedPost.Ru);

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
                    BlogPostId = existing.Id, // Ensure foreign key is set
                    Url = media.Url,
                    MediaType = media.MediaType
                });
            }
        }

        // ✅ CRITICAL: Force EF to recognize changes
        _context.Entry(existing).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    private void CopyTranslation(BlogPostTranslation existing, BlogPostTranslation updated)
    {
        if (updated == null || existing == null) return;

        existing.Title = updated.Title ?? existing.Title;
        existing.Subtitle = updated.Subtitle ?? existing.Subtitle;
        existing.Content = updated.Content ?? existing.Content;
        existing.MetaTitle = updated.MetaTitle ?? existing.MetaTitle;
        existing.MetaDescription = updated.MetaDescription ?? existing.MetaDescription;
        existing.MetaKeywords = updated.MetaKeywords ?? existing.MetaKeywords;
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