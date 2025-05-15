using Microsoft.AspNetCore.Http;
namespace NutvaCms.Application.DTOs;

public class BannerDto
{
    public string Title { get; set; } = null!;
    public string Subtitle { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
    public List<IFormFile>? Images { get; set; } = new();
}
