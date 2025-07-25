namespace NutvaCms.Application.DTOs.BlogMedia;

public class UpdateBlogMediaDto
{
    public Guid Id { get; set; }  // BlogPostMedia.Id
    public string? Caption { get; set; }
    public string? AltText { get; set; }
}
