using NutvaCms.Application.DTOs.BannerDtos;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerSummaryDto>> GetAllAsync(string lang);
        Task<Banner?> GetByIdAsync(Guid id);
        Task<Banner> CreateAsync(CreateBannerDto dto);
        Task<Banner?> UpdateAsync(Guid id, UpdateBannerDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
