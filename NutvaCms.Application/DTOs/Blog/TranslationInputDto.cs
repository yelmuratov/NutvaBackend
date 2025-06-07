namespace NutvaCms.Application.DTOs.Blog;

public class TranslationInputDto
{
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = null!;

    // SEO fields
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
}
