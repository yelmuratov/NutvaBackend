using NutvaCms.Application.DTOs.Blog;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Mappers.Blog;

public static class BlogPostMapper
{
    public static BlogPostDto ToDto(BlogPost entity)
    {
        return new BlogPostDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Published = entity.Published,
            En = MapTranslationOutput(entity.En),
            Uz = MapTranslationOutput(entity.Uz),
            Ru = MapTranslationOutput(entity.Ru),
            Media = entity.Media.Select(m => new MediaInputDto
            {
                Url = m.Url,
                MediaType = m.MediaType.ToString(),
                Caption = m.Caption,
                AltText = m.AltText,
                // Note: Id is not included in MediaInputDto; add it if needed
            }).ToList(),
            ViewCount = entity.ViewCount
        };
    }

    public static BlogPostSummaryDto ToSummaryDto(BlogPost entity, LanguageCode language)
    {
        var translation = language switch
        {
            LanguageCode.En => entity.En,
            LanguageCode.Uz => entity.Uz,
            LanguageCode.Ru => entity.Ru,
            _ => entity.En
        };

        return new BlogPostSummaryDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Published = entity.Published,
            Title = translation.Title,
            Subtitle = translation.Subtitle,
            Content = translation.Content,
            MetaTitle = translation.MetaTitle,
            MetaDescription = translation.MetaDescription,
            MetaKeywords = translation.MetaKeywords,
            Media = entity.Media.Select(m => new MediaInputDto
            {
                Url = m.Url,
                MediaType = m.MediaType.ToString(),
                Caption = m.Caption,
                AltText = m.AltText
            }).ToList(),
            ViewCount = entity.ViewCount
        };
    }

    public static BlogPost FromCreateDto(CreateBlogPostDto dto)
    {
        return new BlogPost
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Published = dto.Published,
            En = MapTranslation(dto.En),
            Uz = MapTranslation(dto.Uz),
            Ru = MapTranslation(dto.Ru),
            Media = new List<BlogPostMedia>() // media now handled separately via BlogMediaController
        };
    }

    public static void ApplyUpdateDto(BlogPost entity, UpdateBlogPostDto dto)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        if (dto.Published.HasValue)
            entity.Published = dto.Published.Value;

        if (dto.En != null)
        {
            if (entity.En == null)
                entity.En = new BlogPostTranslation();

            ApplyTranslationUpdate(entity.En, dto.En);
        }

        if (dto.Uz != null)
        {
            if (entity.Uz == null)
                entity.Uz = new BlogPostTranslation();

            ApplyTranslationUpdate(entity.Uz, dto.Uz);
        }

        if (dto.Ru != null)
        {
            if (entity.Ru == null)
                entity.Ru = new BlogPostTranslation();

            ApplyTranslationUpdate(entity.Ru, dto.Ru);
        }

        // ✅ Media is now handled separately via BlogMediaController — do not modify here
    }

    private static void ApplyTranslationUpdate(BlogPostTranslation existing, TranslationInputDto dto)
    {
        if (dto.Title != null)
            existing.Title = dto.Title;

        if (dto.Subtitle != null)
            existing.Subtitle = dto.Subtitle;

        if (dto.Content != null)
            existing.Content = dto.Content;

        if (dto.MetaTitle != null)
            existing.MetaTitle = dto.MetaTitle;

        if (dto.MetaDescription != null)
            existing.MetaDescription = dto.MetaDescription;

        if (dto.MetaKeywords != null)
            existing.MetaKeywords = dto.MetaKeywords;
    }

    private static BlogPostTranslation MapTranslation(TranslationInputDto dto)
    {
        return new BlogPostTranslation
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Content = dto.Content,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords
        };
    }

    private static TranslationInputDto MapTranslationOutput(BlogPostTranslation? translation)
    {
        if (translation == null)
        {
            return new TranslationInputDto();
        }

        return new TranslationInputDto
        {
            Title = translation.Title,
            Subtitle = translation.Subtitle,
            Content = translation.Content,
            MetaTitle = translation.MetaTitle,
            MetaDescription = translation.MetaDescription,
            MetaKeywords = translation.MetaKeywords
        };
    }
}
