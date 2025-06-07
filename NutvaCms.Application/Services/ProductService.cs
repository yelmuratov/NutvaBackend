using NutvaCms.Application.DTOs.ProductDtos;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

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

    // ✅ GET ALL PRODUCTS with language param
    public async Task<IEnumerable<ProductSummaryDto>> GetAllAsync(string lang)
    {
        var products = await _repo.GetAllAsync();
        var langEnum = Enum.TryParse<LanguageCode>(lang, true, out var parsedLang) ? parsedLang : LanguageCode.En;

        return products.Select(p => ProductMapper.ToSummaryDto(p, langEnum)).ToList();
    }

    // ✅ GET BY ID full product
    public Task<Product?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    // ✅ CREATE PRODUCT
    public async Task<Product> CreateAsync(CreateProductDto dto)
    {
        List<string> imageUrls = new();

        if (dto.Images != null && dto.Images.Any())
        {
            imageUrls = await _fileService.UploadManyAsync(dto.Images);
        }

        var product = ProductMapper.FromCreateDto(dto, imageUrls);
        await _repo.AddAsync(product);
        return product;
    }

    // ✅ UPDATE PRODUCT
    public async Task<Product?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return null;

        List<string> imageUrls = product.ImageUrls;

        if (dto.Images != null && dto.Images.Any())
        {
            imageUrls = await _fileService.UploadManyAsync(dto.Images);
        }

        ProductMapper.ApplyUpdateDto(product, dto, imageUrls);
        await _repo.UpdateAsync(product);
        return product;
    }

    // ✅ DELETE PRODUCT
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
