using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/pixels")]
public class TrackingPixelController : ControllerBase
{
    private readonly ITrackingPixelService _service;

    public TrackingPixelController(ITrackingPixelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddOrUpdate([FromBody] TrackingPixelDto dto)
    {
        await _service.AddOrUpdateAsync(dto);
        return Ok(new { message = "Tracking pixel saved." });
    }
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrackingPixelDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return Ok(new { message = "Tracking pixel updated." });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { message = "Tracking pixel deleted." });
    }

}

