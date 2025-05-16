using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Application.Interfaces;
using System.Security.Claims;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAuthService _authService;

    public AdminController(IAdminService adminService, IAuthService authService)
    {
        _adminService = adminService;
        _authService = authService;
    }

    private Guid? GetCurrentAdminId()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var parsed) ? parsed : null;
    }

    private async Task<bool> IsSuperAdmin()
    {
        var id = GetCurrentAdminId();
        if (id == null) return false;

        var admin = await _authService.GetCurrentAsync(id.Value);
        return admin != null && admin.IsSuperAdmin;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!await IsSuperAdmin())
            return StatusCode(403, new
            {
                error = "Forbidden",
                message = "Only superadmins can view all admins."
            });

        var admins = await _adminService.GetAllAsync();
        return Ok(admins);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminDto dto)
    {
        if (!await IsSuperAdmin())
            return StatusCode(403, new
            {
                error = "Forbidden",
                message = "Only superadmins can create admins."
            });

        var created = await _adminService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AdminDto dto)
    {
        if (!await IsSuperAdmin())
            return StatusCode(403, new
            {
                error = "Forbidden",
                message = "Only superadmins can update admins."
            });

        var updated = await _adminService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await IsSuperAdmin())
            return StatusCode(403, new
            {
                error = "Forbidden",
                message = "Only superadmins can delete admins."
            });

        var deleted = await _adminService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
