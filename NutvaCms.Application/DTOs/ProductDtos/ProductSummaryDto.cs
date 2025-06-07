namespace NutvaCms.Application.DTOs.ProductDtos;

public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
    public decimal Price { get; set; }
    public string Slug { get; set; } = null!;
    public int ViewCount { get; set; }
    public int BuyClickCount { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}
