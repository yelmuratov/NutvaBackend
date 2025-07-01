namespace NutvaCms.Domain.Entities;
public class PurchaseRequestProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
