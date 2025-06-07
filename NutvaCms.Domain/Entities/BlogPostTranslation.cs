namespace NutvaCms.Domain.Entities;

public class BlogPostTranslation
{
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = null!;

    // SEO fields
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
}
