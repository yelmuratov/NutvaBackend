using Microsoft.AspNetCore.Http;
namespace NutvaCms.Application.DTOs.ProductDtos;

public class ProductDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string Slug { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
    public List<IFormFile>? Images { get; set; } = new();
}
