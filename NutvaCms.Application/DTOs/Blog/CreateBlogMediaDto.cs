using Microsoft.AspNetCore.Http;

namespace NutvaCms.Application.DTOs.BlogMedia;

public class CreateBlogMediaDto
{
    public Guid BlogPostId { get; set; }

    public List<IFormFile>? ImageFiles { get; set; } = new();
    public List<IFormFile>? VideoFiles { get; set; } = new();

    public List<string>? ImageUrls { get; set; } = new();
    public List<string>? VideoUrls { get; set; } = new();
}
