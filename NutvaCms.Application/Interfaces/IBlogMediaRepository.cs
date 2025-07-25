using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBlogMediaRepository
{
    Task AddManyAsync(List<BlogPostMedia> media);
    Task UpdateManyAsync(List<BlogPostMedia> media);
    Task DeleteManyAsync(List<Guid> ids);
    Task<List<BlogPostMedia>> GetByIdsAsync(List<Guid> ids);
    Task<List<BlogPostMedia>> GetByPostIdAsync(Guid postId);
    Task<BlogPostMedia?> GetByIdAsync(Guid id);
}
