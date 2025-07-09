namespace NutvaCms.Domain.Entities;

public class PurchaseRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BuyerName { get; set; } = null!;
    public string Region { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; } // ✅ NEW

    // Related order items
    public ICollection<PurchaseRequestProduct> Products { get; set; } = new List<PurchaseRequestProduct>();
}

