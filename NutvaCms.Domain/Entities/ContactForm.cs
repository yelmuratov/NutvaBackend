namespace NutvaCms.Domain.Entities;

public class ContactForm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
