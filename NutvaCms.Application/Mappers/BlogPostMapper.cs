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
                MediaType = m.MediaType.ToString()
            }).ToList()
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
                MediaType = m.MediaType.ToString()
            }).ToList()
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

            Media = new List<BlogPostMedia>() // media handled after file processing in service
        };
    }

    public static void ApplyUpdateDto(BlogPost entity, UpdateBlogPostDto dto)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        entity.En = MapTranslation(dto.En);
        entity.Uz = MapTranslation(dto.Uz);
        entity.Ru = MapTranslation(dto.Ru);
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

    private static TranslationInputDto MapTranslationOutput(BlogPostTranslation translation)
    {
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
