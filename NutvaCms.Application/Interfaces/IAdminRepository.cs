
namespace NutvaCms.Application.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<Admin>> GetAllAsync();
    Task<Admin?> GetByIdAsync(Guid id);
    Task<Admin?> GetByEmailAsync(string email);
    Task AddAsync(Admin admin);
    Task UpdateAsync(Admin admin);
    Task DeleteAsync(Admin admin);
}
