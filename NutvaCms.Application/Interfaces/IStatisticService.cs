using NutvaCms.Application.DTOs;

namespace NutvaCms.Application.Interfaces
{
    public interface IStatisticService
    {
        Task TrackVisitAsync();
        Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto, string lang = "Uz"); // language selection supported
        Task<IEnumerable<PurchaseRequestDto>> GetAllPurchaseRequestsAsync(); // ❗ Changed: returns DTOs, not entities
        Task<IEnumerable<SiteStatisticDto>> GetSiteStatisticsAsync(); // ❗ RECOMMENDED: Use a DTO here too!
    }
}
