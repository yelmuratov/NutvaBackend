using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.Blog;

public class UpdateBlogPostDto
{
    public TranslationInputDto En { get; set; } = null!;
    public TranslationInputDto Uz { get; set; } = null!;
    public TranslationInputDto Ru { get; set; } = null!;
    public List<IFormFile>? ImageFiles { get; set; } = new();
    public List<IFormFile>? VideoFiles { get; set; } = new();
    public bool Published { get; set; } = false;
    public List<string>? ImageUrls { get; set; } = new();
    public List<string>? VideoUrls { get; set; } = new();
}
