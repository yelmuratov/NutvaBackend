using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBlogPostRepository
{
    Task<List<BlogPost>> GetAllAsync();
    Task<BlogPost?> GetByIdAsync(Guid id);
    Task AddAsync(BlogPost blogPost);
    Task UpdateAsync(BlogPost blogPost);
    Task DeleteAsync(BlogPost blogPost);
    Task IncrementViewCountAsync(Guid id);
}
