namespace NutvaCms.Application.DTOs.Blog;

public class BlogPostSummaryDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Published { get; set; }

    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;

    public List<MediaInputDto> Media { get; set; } = new();
}
