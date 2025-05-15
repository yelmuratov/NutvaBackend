using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBlogRepository
{
    Task<IEnumerable<Blog>> GetAllAsync();
    Task<Blog?> GetByIdAsync(Guid id);
    Task AddAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(Blog blog);
}
