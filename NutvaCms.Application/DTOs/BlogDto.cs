namespace NutvaCms.Application.DTOs;

public class BlogDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;

    public List<string>? ImageUrls { get; set; } 
}
