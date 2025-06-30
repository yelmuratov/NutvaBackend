using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces
{
    public interface IStatisticService
    {
        Task TrackVisitAsync();
        Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto, string lang = "Uz"); // now supports language selection
        Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync();
        Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync();
    }
}
