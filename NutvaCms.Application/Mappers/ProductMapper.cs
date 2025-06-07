using NutvaCms.Application.DTOs.ProductDtos;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Mappers
{
    public static class ProductMapper
    {
        public static Product FromCreateDto(CreateProductDto dto, List<string> imageUrls)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Price = dto.Price,
                Slug = dto.Slug,
                ImageUrls = imageUrls,

                En = MapTranslation(dto.En),
                Uz = MapTranslation(dto.Uz),
                Ru = MapTranslation(dto.Ru),

                CreatedAt = DateTime.UtcNow,
                ViewCount = 0,
                BuyClickCount = 0
            };
        }

        public static void ApplyUpdateDto(Product product, UpdateProductDto dto, List<string> imageUrls)
        {
            product.Price = dto.Price;
            product.Slug = dto.Slug;
            product.ImageUrls = imageUrls;

            product.En = MapTranslation(dto.En);
            product.Uz = MapTranslation(dto.Uz);
            product.Ru = MapTranslation(dto.Ru);

            product.UpdatedAt = DateTime.UtcNow;
        }

        public static ProductSummaryDto ToSummaryDto(Product product, LanguageCode lang)
        {
            var translation = lang switch
            {
                LanguageCode.En => product.En,
                LanguageCode.Uz => product.Uz,
                LanguageCode.Ru => product.Ru,
                _ => product.En
            };

            return new ProductSummaryDto
            {
                Id = product.Id,
                Name = translation.Name,
                Description = translation.Description,
                MetaTitle = translation.MetaTitle,
                MetaDescription = translation.MetaDescription,
                MetaKeywords = translation.MetaKeywords,
                Price = product.Price,
                Slug = product.Slug,
                ViewCount = product.ViewCount,
                BuyClickCount = product.BuyClickCount,
                ImageUrls = product.ImageUrls
            };
        }

        private static ProductTranslation MapTranslation(ProductTranslationInputDto dto)
        {
            return new ProductTranslation
            {
                Name = dto.Name,
                Description = dto.Description,
                MetaTitle = dto.MetaTitle,
                MetaDescription = dto.MetaDescription,
                MetaKeywords = dto.MetaKeywords
            };
        }
    }
}
