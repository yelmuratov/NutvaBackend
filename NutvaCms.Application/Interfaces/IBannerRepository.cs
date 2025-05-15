using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBannerRepository
{
    Task<IEnumerable<Banner>> GetAllAsync();
    Task<Banner?> GetByIdAsync(Guid id);
    Task AddAsync(Banner banner);
    Task UpdateAsync(Banner banner);
    Task DeleteAsync(Guid id);
}

