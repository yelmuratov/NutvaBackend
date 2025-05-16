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
    private readonly ITokenBlacklistService _blacklistService;

    public AuthController(IAuthService authService, ITokenBlacklistService blacklistService)
    {
        _blacklistService = blacklistService;
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

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null) return BadRequest("No token found");

        var expClaim = User.FindFirst("exp");
        if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
            return BadRequest("Invalid token expiration");

        var expiration = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        _blacklistService.BlacklistToken(token, expiration);

        return Ok("Logged out successfully");
    }

}
