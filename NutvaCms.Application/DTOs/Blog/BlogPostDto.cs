namespace NutvaCms.Application.DTOs.Blog;

public class BlogPostDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Published { get; set; }
    public TranslationInputDto En { get; set; } = null!;
    public TranslationInputDto Uz { get; set; } = null!;
    public TranslationInputDto Ru { get; set; } = null!;

    public List<MediaInputDto> Media { get; set; } = new();
}
