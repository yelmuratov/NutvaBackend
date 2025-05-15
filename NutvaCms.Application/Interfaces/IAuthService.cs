using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginDto dto);
    Task<Admin?> GetCurrentAsync(Guid adminId);
}
