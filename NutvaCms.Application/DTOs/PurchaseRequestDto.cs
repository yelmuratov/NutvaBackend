namespace NutvaCms.Application.DTOs;

public class PurchaseRequestDto
{
    public Guid ProductId { get; set; }
    public string BuyerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Comment { get; set; }
}
