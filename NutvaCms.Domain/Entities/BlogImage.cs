namespace NutvaCms.Domain.Entities;

public class BlogImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = null!;
    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
}
