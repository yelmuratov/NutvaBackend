namespace NutvaCms.Application.DTOs.Blog;

public class UpdateBlogPostDto
{
    public TranslationInputDto? En { get; set; }
    public TranslationInputDto? Uz { get; set; }
    public TranslationInputDto? Ru { get; set; }

    public bool? Published { get; set; }
}
