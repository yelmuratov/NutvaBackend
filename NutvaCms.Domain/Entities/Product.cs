using NutvaCms.Domain.Enums;

namespace NutvaCms.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ProductTranslation En { get; set; } = new();
    public ProductTranslation Uz { get; set; } = new();
    public ProductTranslation Ru { get; set; } = new();
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; } = 0;
    public int BuyClickCount { get; set; } = 0;
    public List<string> ImageUrls { get; set; } = new();
    public DateTime? UpdatedAt { get; set; }
    public ICollection<ProductBoxPrice> BoxPrices { get; set; } = new List<ProductBoxPrice>();

}