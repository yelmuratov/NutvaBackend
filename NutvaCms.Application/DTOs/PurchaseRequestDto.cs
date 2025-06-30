namespace NutvaCms.Application.DTOs;

public class PurchaseRequestDto
{
    public string BuyerName { get; set; }
    public int Age { get; set; }
    public string ForWhom { get; set; }
    public string Problem { get; set; }
    public string Region { get; set; }
    public string Phone { get; set; }
    public string Comment { get; set; }
    public List<PurchaseProductDto> Products { get; set; }
}


