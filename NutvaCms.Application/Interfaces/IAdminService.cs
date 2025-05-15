using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<Admin>> GetAllAsync();
    Task<Admin> CreateAsync(AdminDto dto);
    Task<Admin?> UpdateAsync(Guid id, AdminDto dto);
    Task<bool> DeleteAsync(Guid id);
}
