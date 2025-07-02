namespace NutvaCms.Application.Services;

using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using System.Text.RegularExpressions;

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
                DiscountLabel = NormalizeLabel(x.DiscountLabel)
            }).ToList();

    public async Task<ProductBoxPriceDto?> GetAsync(Guid id)
    {
        var entity = await _repo.GetAsync(id);
        return entity == null ? null : new ProductBoxPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BoxCount = entity.BoxCount,
            DiscountLabel = NormalizeLabel(entity.DiscountLabel)
        };
    }

    public async Task<ProductBoxPriceDto?> GetByProductAndBoxCountAsync(Guid productId, int boxCount)
    {
        var allDiscounts = await _repo.GetByProductAsync(productId);

        // Find the highest applicable discount that doesn't exceed boxCount
        var matched = allDiscounts
            .Where(x => x.BoxCount <= boxCount)
            .OrderByDescending(x => x.BoxCount)
            .FirstOrDefault();

        if (matched == null)
            return null;

        return new ProductBoxPriceDto
        {
            Id = matched.Id,
            ProductId = matched.ProductId,
            BoxCount = matched.BoxCount,
            DiscountLabel = NormalizeLabel(matched.DiscountLabel)
        };
    }

    public async Task<ProductBoxPriceDto> CreateAsync(CreateProductBoxPriceDto dto)
    {
        var entity = new ProductBoxPrice
        {
            ProductId = dto.ProductId,
            BoxCount = dto.BoxCount,
            DiscountLabel = NormalizeLabel(dto.DiscountLabel)
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
        if (entity == null)
            throw new Exception("Not found!");

        entity.BoxCount = dto.BoxCount;
        entity.DiscountLabel = NormalizeLabel(dto.DiscountLabel);

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

    private string NormalizeLabel(string rawLabel)
    {
        if (string.IsNullOrWhiteSpace(rawLabel)) return "0%";

        var match = Regex.Match(rawLabel, @"\d+");
        if (!match.Success) return "0%";

        int parsed = int.Parse(match.Value);
        parsed = Math.Clamp(parsed, 0, 100);
        return $"{parsed}%";
    }
}
