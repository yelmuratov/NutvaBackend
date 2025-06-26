using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        public ProductTranslationInputDto? En { get; set; }
        public ProductTranslationInputDto? Uz { get; set; }
        public ProductTranslationInputDto? Ru { get; set; }

        public string? Slug { get; set; }
        public decimal? Price { get; set; }

        public List<IFormFile>? Images { get; set; } = new();
    }
}
