using NutvaCms.Domain.Enums;

namespace NutvaCms.Domain.Entities;

public class ProductTranslation
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string MetaTitle { get; set; } = null!;
    public string MetaDescription { get; set; } = null!;
    public string MetaKeywords { get; set; } = null!;
    public string Slug { get; set; } = null!;
}