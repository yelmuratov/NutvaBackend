namespace NutvaCms.Domain.Entities;
public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
