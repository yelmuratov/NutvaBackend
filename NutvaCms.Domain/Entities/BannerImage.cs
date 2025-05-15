namespace NutvaCms.Domain.Entities;

public class BannerImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = null!;
    public Guid BannerId { get; set; }
    public Banner Banner { get; set; } = null!;
}
