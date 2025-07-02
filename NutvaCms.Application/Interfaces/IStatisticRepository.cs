using NutvaCms.Domain.Entities;
using NutvaCms.Application.DTOs;

namespace NutvaCms.Application.Interfaces
{
    public interface IStatisticRepository
    {
        Task TrackVisitAsync();

        // Old method (optional — can be removed if unused)
        Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto);

        // ✅ NEW method: full entity insert (with TotalPrice, DiscountedPrice, etc.)
        Task<bool> AddPurchaseRequestEntityAsync(PurchaseRequest entity);

        Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync();
        Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync();
    }
}
