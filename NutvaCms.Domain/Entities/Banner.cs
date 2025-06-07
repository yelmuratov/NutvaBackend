namespace NutvaCms.Domain.Entities;

public class Banner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BannerTranslation En { get; set; } = new();
    public BannerTranslation Uz { get; set; } = new();
    public BannerTranslation Ru { get; set; } = new();

    public string Link { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new();
}