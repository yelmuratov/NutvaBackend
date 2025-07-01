namespace NutvaCms.Application.DTOs;

public class ContactFormDto
{
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Comment { get; set; }
}
