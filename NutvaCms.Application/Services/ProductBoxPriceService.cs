namespace NutvaCms.Application.Services;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

public class ProductBoxPriceService : IProductBoxPriceService
{
    private readonly IProductBoxPriceRepository _repo;

    public ProductBoxPriceService(IProductBoxPriceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductBoxPriceDto>> GetByProductAsync(Guid productId)
        => (await _repo.GetByProductAsync(productId))
            .Select(x => new ProductBoxPriceDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                BoxCount = x.BoxCount,
                DiscountLabel = x.DiscountLabel
            }).ToList();

    public async Task<ProductBoxPriceDto?> GetAsync(Guid id)
    {
        var entity = await _repo.GetAsync(id);
        return entity == null ? null : new ProductBoxPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BoxCount = entity.BoxCount,
            DiscountLabel = entity.DiscountLabel
        };
    }

    public async Task<ProductBoxPriceDto?> GetByProductAndBoxCountAsync(Guid productId, int boxCount)
    {
        var entity = await _repo.GetByProductAndBoxCountAsync(productId, boxCount);
        return entity == null ? null : new ProductBoxPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BoxCount = entity.BoxCount,
            DiscountLabel = entity.DiscountLabel
        };
    }

    public async Task<ProductBoxPriceDto> CreateAsync(CreateProductBoxPriceDto dto)
    {
        var entity = new ProductBoxPrice
        {
            ProductId = dto.ProductId,
            BoxCount = dto.BoxCount,
            DiscountLabel = dto.DiscountLabel
        };
        await _repo.AddAsync(entity);
        return new ProductBoxPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BoxCount = entity.BoxCount,
            DiscountLabel = entity.DiscountLabel
        };
    }

    public async Task<ProductBoxPriceDto> UpdateAsync(Guid id, UpdateProductBoxPriceDto dto)
    {
        var entity = await _repo.GetAsync(id);
        if (entity == null) throw new Exception("Not found!");
        entity.BoxCount = dto.BoxCount;
        entity.DiscountLabel = dto.DiscountLabel;
        await _repo.UpdateAsync(entity);
        return new ProductBoxPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BoxCount = entity.BoxCount,
            DiscountLabel = entity.DiscountLabel
        };
    }

    public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
}
