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

    public StatisticsController(
        IStatisticService service)
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
        // 1. Validate input
        if (string.IsNullOrWhiteSpace(dto.BuyerName) || string.IsNullOrWhiteSpace(dto.Phone))
        {
            return BadRequest(new { message = "Buyer name and phone are required." });
        }
        if (dto.Products == null || dto.Products.Count == 0)
        {
            return BadRequest(new { message = "At least one product must be specified in the order." });
        }

        // 2. Log received DTO
        Console.WriteLine($"Received purchase request: {System.Text.Json.JsonSerializer.Serialize(dto)}");

        // 3. Save to DB & send Telegram notification
        var success = await _service.AddPurchaseRequestAsync(dto);
        if (!success)
            return NotFound(new { message = "One or more products do not exist." });

        // 4. Done
        return Ok(new { message = "Request submitted" });
    }

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
