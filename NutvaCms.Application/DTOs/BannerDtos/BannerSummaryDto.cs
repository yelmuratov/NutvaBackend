namespace NutvaCms.Application.DTOs.BannerDtos;

public class BannerSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Subtitle { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
    public string Link { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new();
}
