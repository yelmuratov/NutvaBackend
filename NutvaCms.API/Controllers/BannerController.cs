using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    // Public access - anyone can fetch banners
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var banners = await _bannerService.GetAllAsync();
        return Ok(banners);
    }

    // Public access - anyone can view a specific banner
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var banner = await _bannerService.GetByIdAsync(id);
        return banner is null ? NotFound() : Ok(banner);
    }

    // Authenticated admins only - create a banner
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] BannerDto dto)
    {
        var created = await _bannerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // Authenticated admins only - update a banner
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] BannerDto dto)
    {
        var updated = await _bannerService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // Authenticated admins only - delete a banner
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _bannerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
