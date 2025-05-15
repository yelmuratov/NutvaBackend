namespace NutvaCms.Application.DTOs;
public class TrackingPixelDto
{
    public string Platform { get; set; } = null!; // "Meta", "Yandex", "Google"
    public string Script { get; set; } = null!;
}
