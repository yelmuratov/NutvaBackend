using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Product>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Product?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Product> CreateAsync(ProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Slug = dto.Slug,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            Images = dto.ImageUrls?.Select(url => new ProductImage { ImageUrl = url }).ToList() ?? new()
        };

        await _repo.AddAsync(product);
        return product;
    }

    public async Task<Product?> UpdateAsync(Guid id, ProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Slug = dto.Slug;
        product.MetaTitle = dto.MetaTitle;
        product.MetaDescription = dto.MetaDescription;
        product.MetaKeywords = dto.MetaKeywords;
        product.Images = dto.ImageUrls?.Select(url => new ProductImage { ImageUrl = url }).ToList() ?? new();

        await _repo.UpdateAsync(product);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        await _repo.DeleteAsync(product);
        return true;
    }
}
