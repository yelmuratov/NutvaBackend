namespace NutvaCms.Application.DTOs;

public class ProductBoxPriceDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int BoxCount { get; set; }
    public string DiscountLabel { get; set; } = "0%";

    // This exposes a clean, validated percent you can safely use
    public int DiscountPercent => ParsePercent(DiscountLabel);

    private int ParsePercent(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) return 0;

        var match = System.Text.RegularExpressions.Regex.Match(label, @"\d+");
        if (!match.Success) return 0;

        int percent = int.Parse(match.Value);
        return percent >= 0 && percent <= 100 ? percent : 0;
    }
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
