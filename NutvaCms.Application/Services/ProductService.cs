using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IFileService _fileService;

    public ProductService(IProductRepository repo, IFileService fileService)
    {
        _repo = repo;
        _fileService = fileService;
    }

    public Task<IEnumerable<Product>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Product?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Product> CreateAsync(ProductDto dto)
    {
        var urls = dto.Images != null && dto.Images.Any()
            ? await _fileService.UploadManyAsync(dto.Images)
            : new List<string>();

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Slug = dto.Slug,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            ImageUrls = urls
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

        if (dto.Images != null && dto.Images.Any())
        {
            var urls = await _fileService.UploadManyAsync(dto.Images);
            product.ImageUrls = urls;
        }

        await _repo.UpdateAsync(product);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        if (product.ImageUrls.Any())
        {
            foreach (var imageUrl in product.ImageUrls)
            {
                await _fileService.DeleteManyAsync(imageUrl);
            }
        }

        await _repo.DeleteAsync(product);
        return true;
    }

    public async Task IncrementViewAsync(Guid id)
    {
        await _repo.IncrementProductViewAsync(id);
    }

    public async Task IncrementBuyClickAsync(Guid id)
    {
        await _repo.IncrementProductBuyClickAsync(id);
    }

}
