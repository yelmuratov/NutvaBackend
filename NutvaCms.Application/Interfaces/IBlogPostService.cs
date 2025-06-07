using NutvaCms.Application.DTOs.Blog;

namespace NutvaCms.Application.Interfaces;

public interface IBlogPostService
{
    Task<List<BlogPostSummaryDto>> GetAllAsync(string language);
    Task<BlogPostDto?> GetByIdAsync(Guid id);
    Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto);
    Task UpdateAsync(Guid id, UpdateBlogPostDto dto);
    Task DeleteAsync(Guid id);
}
