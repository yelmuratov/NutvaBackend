using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{

    public interface IProductBoxPriceRepository
    {
        Task<List<ProductBoxPrice>> GetByProductAsync(Guid productId);
        Task<ProductBoxPrice?> GetAsync(Guid id);
        Task<ProductBoxPrice?> GetByProductAndBoxCountAsync(Guid productId, int boxCount);
        Task AddAsync(ProductBoxPrice entity);
        Task UpdateAsync(ProductBoxPrice entity);
        Task DeleteAsync(Guid id);
    }
}
