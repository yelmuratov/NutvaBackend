using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.ProductDtos;

public class CreateProductDto
{
    public ProductTranslationInputDto En { get; set; } = null!;
    public ProductTranslationInputDto Uz { get; set; } = null!;
    public ProductTranslationInputDto Ru { get; set; } = null!;

    public decimal Price { get; set; }
    public List<IFormFile>? Images { get; set; } = new();
}
