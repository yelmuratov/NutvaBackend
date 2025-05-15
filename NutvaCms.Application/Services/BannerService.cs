using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class BannerService : IBannerService
{
    private readonly IBannerRepository _repo;
    private readonly IFileService _fileService;

    public BannerService(IBannerRepository repo, IFileService fileService)
    {
        _repo = repo;
        _fileService = fileService;
    }

    public Task<IEnumerable<Banner>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Banner?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Banner> CreateAsync(BannerDto dto)
    {
        var urls = dto.Images != null && dto.Images.Any()
            ? await _fileService.UploadManyAsync(dto.Images)
            : new List<string>();

        var banner = new Banner
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Link = dto.Link,
            MetaTitle = dto.MetaTitle,
            MetaKeywords = dto.MetaKeywords,
            MetaDescription = dto.MetaDescription,
            ImageUrls = urls
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
        banner.MetaKeywords = dto.MetaKeywords;
        banner.MetaDescription = dto.MetaDescription;

        if (dto.Images != null && dto.Images.Any())
        {
            var urls = await _fileService.UploadManyAsync(dto.Images);
            banner.ImageUrls = urls;
        }

        await _repo.UpdateAsync(banner);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var banner = await _repo.GetByIdAsync(id);
        if (banner == null) return false;

        if (banner.ImageUrls.Any())
        {
            foreach (var imageUrl in banner.ImageUrls)
            {
                await _fileService.DeleteManyAsync(imageUrl);
            }
        }

        await _repo.DeleteAsync(id);
        return true;
    }
}
