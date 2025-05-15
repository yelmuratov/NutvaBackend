using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(ProductDto dto);
    Task<Product?> UpdateAsync(Guid id, ProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}
