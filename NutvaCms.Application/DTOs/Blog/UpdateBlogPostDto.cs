using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.Blog;

public class UpdateBlogPostDto
{
    public TranslationInputDto? En { get; set; }
    public TranslationInputDto? Uz { get; set; }
    public TranslationInputDto? Ru { get; set; }

    public List<IFormFile>? ImageFiles { get; set; } = new();
    public List<IFormFile>? VideoFiles { get; set; } = new();

    public bool? Published { get; set; }  // Changed to nullable
    public List<string>? ImageUrls { get; set; } = new();
    public List<string>? VideoUrls { get; set; } = new();
}
