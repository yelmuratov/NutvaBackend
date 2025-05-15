using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class StatisticService : IStatisticService
{
    private readonly IStatisticRepository _repo;
    private readonly IBitrixService _bitrix;

    public StatisticService(IStatisticRepository repo, IBitrixService bitrix)
    {
        _repo = repo;
        _bitrix = bitrix;
    }

    public Task TrackVisitAsync() => _repo.TrackVisitAsync();

    public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto)
    {
        var success = await _repo.AddPurchaseRequestAsync(dto);
        if (!success) return false;

        await _bitrix.SendLeadAsync(new BitrixLeadDto
        {
            Name = dto.BuyerName,
            Phone = dto.Phone,
            Comment = dto.Comment ?? ""
        });

        return true;
    }

    public Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync() =>
        _repo.GetAllPurchaseRequestsAsync();
    
    public Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync()
{
    return _repo.GetSiteStatisticsAsync();
}
}
