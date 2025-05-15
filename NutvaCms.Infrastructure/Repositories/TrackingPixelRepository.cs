using Microsoft.EntityFrameworkCore;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class TrackingPixelRepository : ITrackingPixelRepository
{
    private readonly AppDbContext _context;

    public TrackingPixelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrackingPixel>> GetAllAsync()
    {
        return await _context.TrackingPixels.ToListAsync();
    }

    public async Task AddOrUpdateAsync(TrackingPixelDto dto)
    {
        var existing = await _context.TrackingPixels
            .FirstOrDefaultAsync(p => p.Platform == dto.Platform);

        if (existing is not null)
        {
            existing.Script = dto.Script;
        }
        else
        {
            var pixel = new TrackingPixel
            {
                Platform = dto.Platform,
                Script = dto.Script
            };
            await _context.TrackingPixels.AddAsync(pixel);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, TrackingPixelDto dto)
    {
        var pixel = await _context.TrackingPixels.FindAsync(id);
        if (pixel is null) return;

        pixel.Platform = dto.Platform;
        pixel.Script = dto.Script;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var pixel = await _context.TrackingPixels.FindAsync(id);
        if (pixel is null) return;

        _context.TrackingPixels.Remove(pixel);
        await _context.SaveChangesAsync();
    }

}
