namespace NutvaCms.Domain.Entities;

public class TrackingPixel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Platform { get; set; } = null!; // "Meta", "Yandex", "Google"
    public string Script { get; set; } = null!;   // raw JS snippet or ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
