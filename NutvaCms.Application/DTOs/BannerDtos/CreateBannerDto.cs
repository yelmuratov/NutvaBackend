using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.BannerDtos;

public class CreateBannerDto
{
    public BannerTranslationInputDto En { get; set; } = null!;
    public BannerTranslationInputDto Uz { get; set; } = null!;
    public BannerTranslationInputDto Ru { get; set; } = null!;
    public string Link { get; set; } = null!;
    public List<IFormFile>? Images { get; set; } = new();
}
