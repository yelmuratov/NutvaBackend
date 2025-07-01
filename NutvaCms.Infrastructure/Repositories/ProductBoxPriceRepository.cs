namespace NutvaCms.Infrastructure.Repositories;

using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

public class ProductBoxPriceRepository : IProductBoxPriceRepository
{
    private readonly AppDbContext _context;
    public ProductBoxPriceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductBoxPrice>> GetByProductAsync(Guid productId)
        => await _context.ProductBoxPrices.Where(x => x.ProductId == productId).ToListAsync();

    public async Task<ProductBoxPrice?> GetAsync(Guid id)
        => await _context.ProductBoxPrices.FindAsync(id);

    public async Task<ProductBoxPrice?> GetByProductAndBoxCountAsync(Guid productId, int boxCount)
        => await _context.ProductBoxPrices.FirstOrDefaultAsync(x => x.ProductId == productId && x.BoxCount == boxCount);

    public async Task AddAsync(ProductBoxPrice entity)
    {
        _context.ProductBoxPrices.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductBoxPrice entity)
    {
        _context.ProductBoxPrices.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.ProductBoxPrices.FindAsync(id);
        if (entity != null)
        {
            _context.ProductBoxPrices.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
