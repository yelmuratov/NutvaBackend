using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
using NutvaCms.Application.Interfaces;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _db.Products.ToListAsync(); // No .Include(p => p.Images)

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        var existing = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existing is null)
            throw new InvalidOperationException($"Product with ID {product.Id} not found");

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.MetaTitle = product.MetaTitle;
        existing.MetaDescription = product.MetaDescription;
        existing.ImageUrls = product.ImageUrls;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }

    public async Task IncrementProductViewAsync(Guid productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product is not null)
        {
            product.ViewCount++;
            await _db.SaveChangesAsync();
        }
    }

    public async Task IncrementProductBuyClickAsync(Guid productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product is not null)
        {
            product.BuyClickCount++;
            await _db.SaveChangesAsync();
        }
    }

}
