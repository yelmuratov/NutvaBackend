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

    // ✅ Legacy version - still usable
    public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto)
    {
        if (dto.Products == null || dto.Products.Count == 0)
            return false;

        var productIds = dto.Products.Select(p => p.ProductId).ToList();
        var dbProductIds = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        if (dbProductIds.Count != productIds.Count)
            return false;

        var request = new PurchaseRequest
        {
            BuyerName = dto.BuyerName,
            Region = dto.Region,
            Phone = dto.Phone,
            Comment = dto.Comment ?? string.Empty,
            Products = dto.Products.Select(p => new PurchaseRequestProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };

        await _context.PurchaseRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ New method to persist full PurchaseRequest entity (already calculated)
    public async Task<bool> AddPurchaseRequestEntityAsync(PurchaseRequest entity)
    {
        if (entity.Products == null || !entity.Products.Any())
            return false;

        await _context.PurchaseRequests.AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync()
    {
        return await _context.PurchaseRequests
            .Include(p => p.Products)
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
