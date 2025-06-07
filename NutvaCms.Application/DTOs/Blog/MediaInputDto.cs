using Microsoft.AspNetCore.Http;
namespace NutvaCms.Application.DTOs.Blog;

public class MediaInputDto
{
    public string? Url { get; set; } 

    public IFormFile? File { get; set; }

    public string? Caption { get; set; }
    public string? AltText { get; set; }

    public string MediaType { get; set; } = null!;  // Enum: Image, ImageUrl, Video, VideoUrl, YoutubeUrl
}
