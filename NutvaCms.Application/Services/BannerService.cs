using NutvaCms.Application.DTOs.BannerDtos;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repo;
        private readonly IFileService _fileService;

        public BannerService(IBannerRepository repo, IFileService fileService)
        {
            _repo = repo;
            _fileService = fileService;
        }

        // ✅ Get all banners with language
        public async Task<IEnumerable<BannerSummaryDto>> GetAllAsync(string lang)
        {
            var banners = await _repo.GetAllAsync();
            var langEnum = Enum.TryParse<LanguageCode>(lang, true, out var parsedLang) ? parsedLang : LanguageCode.En;

            return banners.Select(b => BannerMapper.ToSummaryDto(b, langEnum)).ToList();
        }

        // ✅ Get banner by id (full object without language filter)
        public Task<Banner?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        // ✅ Create banner
        public async Task<Banner> CreateAsync(CreateBannerDto dto)
        {
            var imageUrls = dto.Images != null && dto.Images.Any()
                ? await _fileService.UploadManyAsync(dto.Images)
                : new List<string>();

            var banner = BannerMapper.FromCreateDto(dto, imageUrls);
            await _repo.AddAsync(banner);
            return banner;
        }

        // ✅ Update banner
        public async Task<Banner?> UpdateAsync(Guid id, UpdateBannerDto dto)
        {
            var banner = await _repo.GetByIdAsync(id);
            if (banner == null)
                return null;

            var imageUrls = dto.Images != null && dto.Images.Any()
                ? await _fileService.UploadManyAsync(dto.Images)
                : banner.ImageUrls;

            BannerMapper.ApplyUpdateDto(banner, dto, imageUrls);
            await _repo.UpdateAsync(banner);
            return banner;
        }

        // ✅ Delete banner
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
}
