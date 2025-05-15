namespace NutvaCms.Domain.Entities;

public class SiteStatistic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    public int TotalVisits { get; set; } = 0;
}
