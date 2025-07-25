using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.DTOs.BlogMedia;

public class BlogMediaResponseDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public string? AltText { get; set; }
    public MediaType MediaType { get; set; }
}
