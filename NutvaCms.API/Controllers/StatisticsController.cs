using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticService _service;

    public StatisticsController(IStatisticService service)
    {
        _service = service;
    }

    [HttpPost("track-visit")]
    public async Task<IActionResult> TrackVisit()
    {
        await _service.TrackVisitAsync();
        return Ok(new { message = "Visit tracked" });
    }

    [HttpPost("purchase-request")]
    public async Task<IActionResult> SubmitPurchase([FromBody] PurchaseRequestDto dto)
    {
        var success = await _service.AddPurchaseRequestAsync(dto);
        if (!success)
            return NotFound($"Product with ID {dto.ProductId} does not exist.");

        return Ok(new { message = "Request submitted" });
    }

    // ✅ GET: All purchase requests
    // Only logged-in admins can view all purchase requests
    [Authorize]
    [HttpGet("purchase-requests")]
    public async Task<IActionResult> GetAllPurchaseRequests()
    {
        var data = await _service.GetAllPurchaseRequestsAsync();
        return Ok(data);
    }

    [HttpGet("visits")]
    public async Task<IActionResult> GetVisits()
    {
        var result = await _service.GetSiteStatisticsAsync();
        return Ok(result);
    }

}
