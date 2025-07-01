namespace NutvaCms.Domain.Entities;
public class ProductBoxPrice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int BoxCount { get; set; }
    public string DiscountLabel { get; set; } = "0%"; // e.g. "0%", "15%", "60%"
}

