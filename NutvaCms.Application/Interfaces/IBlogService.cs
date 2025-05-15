using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetAllAsync();
    Task<Blog?> GetByIdAsync(Guid id);
    Task<Blog> CreateAsync(BlogDto dto);
    Task<Blog?> UpdateAsync(Guid id, BlogDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task IncrementViewAsync(Guid id);
}
