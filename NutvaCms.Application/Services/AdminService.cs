using NutvaCms.Application.DTOs.Auth;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _repo;

    public AdminService(IAdminRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Admin>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<Admin> CreateAsync(AdminDto dto)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var admin = new Admin
        {
            Email = dto.Email,
            Password = hashedPassword,
            IsSuperAdmin = false // superadmin can only be created manually
        };

        await _repo.AddAsync(admin);
        return admin;
    }

    public async Task<Admin?> UpdateAsync(Guid id, AdminDto dto)
    {
        var admin = await _repo.GetByIdAsync(id);
        if (admin == null) return null;

        admin.Email = dto.Email;
        admin.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _repo.UpdateAsync(admin);
        return admin;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var admin = await _repo.GetByIdAsync(id);
        if (admin == null) return false;

        await _repo.DeleteAsync(admin);
        return true;
    }
}
