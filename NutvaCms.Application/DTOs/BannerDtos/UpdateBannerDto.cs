using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.BannerDtos;

public class UpdateBannerDto
{
    public BannerTranslationInputDto? En { get; set; }
    public BannerTranslationInputDto? Uz { get; set; }
    public BannerTranslationInputDto? Ru { get; set; }
    public string? Link { get; set; }
    public List<IFormFile>? Images { get; set; } = new();
}
