using NutvaCms.Application.DTOs.ProductDtos;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductSummaryDto>> GetAllAsync(string lang);

    Task<Product?> GetByIdAsync(Guid id);

    Task<Product> CreateAsync(CreateProductDto dto);
    Task<Product?> UpdateAsync(Guid id, UpdateProductDto dto);

    Task<bool> DeleteAsync(Guid id);

    Task IncrementViewAsync(Guid id);
    Task IncrementBuyClickAsync(Guid id);
}
