using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBannerService
{
    Task<IEnumerable<Banner>> GetAllAsync();
    Task<Banner?> GetByIdAsync(Guid id);
    Task<Banner> CreateAsync(BannerDto dto);
    Task<Banner?> UpdateAsync(Guid id, BannerDto dto);
    Task<bool> DeleteAsync(Guid id);
}
