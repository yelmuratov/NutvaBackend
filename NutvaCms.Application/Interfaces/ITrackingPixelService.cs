using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface ITrackingPixelService
{
    Task<IEnumerable<TrackingPixel>> GetAllAsync();
    Task AddOrUpdateAsync(TrackingPixelDto dto);
    Task UpdateAsync(Guid id, TrackingPixelDto dto);
    Task DeleteAsync(Guid id);

}

