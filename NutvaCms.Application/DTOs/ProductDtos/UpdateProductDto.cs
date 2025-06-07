using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        // Multilingual translations (required)
        public ProductTranslationInputDto En { get; set; } = null!;
        public ProductTranslationInputDto Uz { get; set; } = null!;
        public ProductTranslationInputDto Ru { get; set; } = null!;

        // Product fields
        public string Slug { get; set; } = null!; // Unique slug for SEO-friendly URLs
        public decimal Price { get; set; }

        // Media: file uploads for product images
        public List<IFormFile>? Images { get; set; } = new();
    }
}
