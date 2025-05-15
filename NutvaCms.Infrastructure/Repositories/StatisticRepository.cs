using Microsoft.EntityFrameworkCore;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class StatisticRepository : IStatisticRepository
{
    private readonly AppDbContext _context;

    public StatisticRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task TrackVisitAsync()
    {
        var today = DateTime.UtcNow.Date;
        var stat = await _context.SiteStatistics.FirstOrDefaultAsync(s => s.Date == today);

        if (stat == null)
        {
            stat = new SiteStatistic { Date = today, TotalVisits = 1 };
            await _context.SiteStatistics.AddAsync(stat);
        }
        else
        {
            stat.TotalVisits += 1;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
            return false;

        var request = new PurchaseRequest
        {
            ProductId = dto.ProductId,
            BuyerName = dto.BuyerName,
            Phone = dto.Phone,
            Comment = dto.Comment ?? string.Empty
        };

        await _context.PurchaseRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync()
    {
        return await _context.PurchaseRequests
            .Include(p => p.Product)
            .OrderByDescending(p => p.SubmittedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync()
    {
        return await _context.SiteStatistics
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }
}
