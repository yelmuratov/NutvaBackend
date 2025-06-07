using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.BannerDtos;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;
using NutvaCms.Domain.Enums;

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

    // ✅ Public access - fetch banners with language param
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string lang = "en")
    {
        var banners = await _bannerService.GetAllAsync(lang);
        return Ok(banners);
    }

    // ✅ Public access - fetch single banner with language param
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] string lang = "en")
    {
        var banner = await _bannerService.GetByIdAsync(id);
        if (banner is null) return NotFound();

        var langEnum = Enum.TryParse<LanguageCode>(lang, true, out var parsedLang) ? parsedLang : LanguageCode.En;
        return Ok(BannerMapper.ToSummaryDto(banner, langEnum));
    }

    // ✅ Authenticated admins only - create banner
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateBannerDto dto)
    {
        var created = await _bannerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ✅ Authenticated admins only - update banner
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateBannerDto dto)
    {
        var updated = await _bannerService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // ✅ Authenticated admins only - delete banner
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _bannerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
