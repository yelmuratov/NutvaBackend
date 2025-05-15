using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        return token is null ? Unauthorized("Invalid credentials") : Ok(new { token });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(adminId) || !Guid.TryParse(adminId, out var id))
            return Unauthorized("Invalid token");

        var admin = await _authService.GetCurrentAsync(id);
        return admin is null ? NotFound() : Ok(admin);
    }
}
