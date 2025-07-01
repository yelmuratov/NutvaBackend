using NutvaCms.Application.DTOs;

namespace NutvaCms.Application.Interfaces
{
    public interface IProductBoxPriceService
    {
        Task<List<ProductBoxPriceDto>> GetByProductAsync(Guid productId);
        Task<ProductBoxPriceDto?> GetAsync(Guid id);
        Task<ProductBoxPriceDto?> GetByProductAndBoxCountAsync(Guid productId, int boxCount);
        Task<ProductBoxPriceDto> CreateAsync(CreateProductBoxPriceDto dto);
        Task<ProductBoxPriceDto> UpdateAsync(Guid id, UpdateProductBoxPriceDto dto);
        Task DeleteAsync(Guid id);
    }
}
