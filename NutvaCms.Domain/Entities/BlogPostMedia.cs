using NutvaCms.Domain.Enums;

namespace NutvaCms.Domain.Entities;

public class BlogPostMedia
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    public MediaType MediaType { get; set; } 
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public string? AltText { get; set; }
}
