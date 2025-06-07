namespace NutvaCms.Domain.Entities;

public class BlogPost
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Published { get; set; }

    // 3 Language Translations:
    public BlogPostTranslation En { get; set; } = new BlogPostTranslation();
    public BlogPostTranslation Uz { get; set; } = new BlogPostTranslation();
    public BlogPostTranslation Ru { get; set; } = new BlogPostTranslation();

    // Media collection
    public ICollection<BlogPostMedia> Media { get; set; } = new List<BlogPostMedia>();
}
