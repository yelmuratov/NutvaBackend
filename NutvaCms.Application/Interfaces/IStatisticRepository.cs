using NutvaCms.Domain.Entities;
using NutvaCms.Application.DTOs;

namespace NutvaCms.Application.Interfaces;

public interface IStatisticRepository
{
    Task TrackVisitAsync();
    Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto); // updated return type
    Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync();
    Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync();
}

