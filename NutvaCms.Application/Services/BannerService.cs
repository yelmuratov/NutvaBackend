using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class BannerService : IBannerService
{
    private readonly IBannerRepository _repo;

    public BannerService(IBannerRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Banner>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Banner?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Banner> CreateAsync(BannerDto dto)
    {
        var banner = new Banner
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Link = dto.Link,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            Images = dto.ImageUrls?.Select(url => new BannerImage { ImageUrl = url }).ToList() ?? new()
        };

        await _repo.AddAsync(banner);
        return banner;
    }

    public async Task<Banner?> UpdateAsync(Guid id, BannerDto dto)
    {
        var banner = await _repo.GetByIdAsync(id);
        if (banner == null) return null;

        banner.Title = dto.Title;
        banner.Subtitle = dto.Subtitle;
        banner.Link = dto.Link;
        banner.MetaTitle = dto.MetaTitle;
        banner.MetaDescription = dto.MetaDescription;
        banner.MetaKeywords = dto.MetaKeywords;
        banner.Images = dto.ImageUrls?.Select(url => new BannerImage { ImageUrl = url }).ToList() ?? new();

        await _repo.UpdateAsync(banner);
        return banner;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var banner = await _repo.GetByIdAsync(id);
        if (banner == null) return false;

        await _repo.DeleteAsync(banner);
        return true;
    }
}
