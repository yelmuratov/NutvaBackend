using Microsoft.EntityFrameworkCore;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class BlogMediaRepository : IBlogMediaRepository
{
    private readonly AppDbContext _context;

    public BlogMediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddManyAsync(List<BlogPostMedia> media)
    {
        if (media == null || !media.Any()) return;

        await _context.BlogPostMedia.AddRangeAsync(media);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateManyAsync(List<BlogPostMedia> media)
    {
        if (media == null || !media.Any()) return;

        _context.BlogPostMedia.UpdateRange(media);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteManyAsync(List<Guid> ids)
    {
        var mediaToDelete = await _context.BlogPostMedia
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();

        if (mediaToDelete.Any())
        {
            _context.BlogPostMedia.RemoveRange(mediaToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<BlogPostMedia>> GetByIdsAsync(List<Guid> ids)
    {
        return await _context.BlogPostMedia
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<List<BlogPostMedia>> GetByPostIdAsync(Guid postId)
    {
        return await _context.BlogPostMedia
            .Where(m => m.BlogPostId == postId)
            .ToListAsync();
    }

    public async Task<BlogPostMedia?> GetByIdAsync(Guid id)
    {
        return await _context.BlogPostMedia
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
