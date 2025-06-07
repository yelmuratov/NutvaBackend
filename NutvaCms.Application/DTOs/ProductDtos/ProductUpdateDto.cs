using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.ProductDtos;

public class ProductUpdateDto
{
    public ProductTranslationInputDto En { get; set; } = null!;
    public ProductTranslationInputDto Uz { get; set; } = null!;
    public ProductTranslationInputDto Ru { get; set; } = null!;

    public decimal Price { get; set; }
    public string Slug { get; set; } = null!;
    public List<IFormFile>? Images { get; set; } = new();
}
