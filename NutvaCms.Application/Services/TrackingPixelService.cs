using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class TrackingPixelService : ITrackingPixelService
{
    private readonly ITrackingPixelRepository _repo;

    public TrackingPixelService(ITrackingPixelRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<TrackingPixel>> GetAllAsync() => _repo.GetAllAsync();

    public Task AddOrUpdateAsync(TrackingPixelDto dto) => _repo.AddOrUpdateAsync(dto);
    public Task UpdateAsync(Guid id, TrackingPixelDto dto) =>
    _repo.UpdateAsync(id, dto);

    public Task DeleteAsync(Guid id) =>
        _repo.DeleteAsync(id);

}