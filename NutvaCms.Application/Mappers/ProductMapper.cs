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
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (dto.Images != null && dto.Images.Any())
                product.ImageUrls = imageUrls;

            if (dto.En != null)
                ApplyTranslationUpdate(product.En, dto.En);

            if (dto.Uz != null)
                ApplyTranslationUpdate(product.Uz, dto.Uz);

            if (dto.Ru != null)
                ApplyTranslationUpdate(product.Ru, dto.Ru);

            product.UpdatedAt = DateTime.UtcNow;
        }

        private static void ApplyTranslationUpdate(ProductTranslation existing, ProductTranslationInputDto incoming)
        {
            if (incoming.Name != null)
                existing.Name = incoming.Name;

            if (incoming.Description != null)
                existing.Description = incoming.Description;

            if (incoming.MetaTitle != null)
                existing.MetaTitle = incoming.MetaTitle;

            if (incoming.MetaDescription != null)
                existing.MetaDescription = incoming.MetaDescription;

            if (incoming.MetaKeywords != null)
                existing.MetaKeywords = incoming.MetaKeywords;

            if (incoming.Slug != null)
                existing.Slug = incoming.Slug; // <-- Now supports per-language slug
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
                Slug = translation.Slug, // <-- Localized slug!
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
                MetaKeywords = dto.MetaKeywords,
                Slug = dto.Slug // <-- Localized slug on creation
            };
        }
    }
}
