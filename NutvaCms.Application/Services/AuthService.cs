using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _repo;
    private readonly IConfiguration _config;

    public AuthService(IAdminRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var admin = await _repo.GetByEmailAsync(dto.Email);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Email, admin.Email)
        }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Admin?> GetCurrentAsync(Guid adminId)
    {
        return await _repo.GetByIdAsync(adminId);
    }
}
