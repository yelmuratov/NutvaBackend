using NutvaCms.Application.DTOs.BannerDtos;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Mappers;

public static class BannerMapper
{
    public static Banner FromCreateDto(CreateBannerDto dto, List<string> imageUrls)
    {
        return new Banner
        {
            Link = dto.Link,
            ImageUrls = imageUrls,
            En = MapTranslation(dto.En),
            Uz = MapTranslation(dto.Uz),
            Ru = MapTranslation(dto.Ru)
        };
    }

    public static void ApplyUpdateDto(Banner banner, UpdateBannerDto dto, List<string> imageUrls)
    {
        if (dto.Link != null)
            banner.Link = dto.Link;

        if (dto.En != null)
            ApplyTranslationUpdate(banner.En, dto.En);

        if (dto.Uz != null)
            ApplyTranslationUpdate(banner.Uz, dto.Uz);

        if (dto.Ru != null)
            ApplyTranslationUpdate(banner.Ru, dto.Ru);

        if (dto.Images != null && dto.Images.Any())
            banner.ImageUrls = imageUrls;
    }

    private static void ApplyTranslationUpdate(BannerTranslation existing, BannerTranslationInputDto incoming)
    {
        if (incoming.Title != null)
            existing.Title = incoming.Title;

        if (incoming.Subtitle != null)
            existing.Subtitle = incoming.Subtitle;

        if (incoming.MetaTitle != null)
            existing.MetaTitle = incoming.MetaTitle;

        if (incoming.MetaDescription != null)
            existing.MetaDescription = incoming.MetaDescription;

        if (incoming.MetaKeywords != null)
            existing.MetaKeywords = incoming.MetaKeywords;
    }

    public static BannerSummaryDto ToSummaryDto(Banner banner, LanguageCode lang)
    {
        var translation = lang switch
        {
            LanguageCode.En => banner.En,
            LanguageCode.Uz => banner.Uz,
            LanguageCode.Ru => banner.Ru,
            _ => banner.En
        };

        return new BannerSummaryDto
        {
            Id = banner.Id,
            Title = translation.Title,
            Subtitle = translation.Subtitle,
            MetaTitle = translation.MetaTitle,
            MetaDescription = translation.MetaDescription,
            MetaKeywords = translation.MetaKeywords,
            Link = banner.Link,
            ImageUrls = banner.ImageUrls
        };
    }

    private static BannerTranslation MapTranslation(BannerTranslationInputDto dto)
    {
        return new BannerTranslation
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords
        };
    }
}
