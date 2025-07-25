using NutvaCms.Application.DTOs.BlogMedia;

namespace NutvaCms.Application.Interfaces;

public interface IBlogMediaService
{
    Task<List<BlogMediaResponseDto>> CreateAsync(CreateBlogMediaDto dto);
    Task UpdateAsync(List<UpdateBlogMediaDto> dtoList);
    Task DeleteAsync(List<Guid> ids);
    Task<BlogMediaResponseDto?> GetByIdAsync(Guid id);
    Task<List<BlogMediaResponseDto>> GetByPostIdAsync(Guid postId);
}
