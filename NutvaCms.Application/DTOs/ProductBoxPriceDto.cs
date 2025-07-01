namespace NutvaCms.Application.DTOs;
public class ProductBoxPriceDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int BoxCount { get; set; }
    public string DiscountLabel { get; set; }
}

public class CreateProductBoxPriceDto
{
    public Guid ProductId { get; set; }
    public int BoxCount { get; set; }
    public string DiscountLabel { get; set; }
}

public class UpdateProductBoxPriceDto
{
    public int BoxCount { get; set; }
    public string DiscountLabel { get; set; }
}
