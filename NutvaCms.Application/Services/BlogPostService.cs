using NutvaCms.Application.DTOs.Blog;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers.Blog;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _repository;

    public BlogPostService(IBlogPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BlogPostSummaryDto>> GetAllAsync(string language)
    {
        var blogs = await _repository.GetAllAsync();
        var langEnum = Enum.TryParse<LanguageCode>(language, true, out var parsedLang) ? parsedLang : LanguageCode.En;
        return blogs.Select(blog => BlogPostMapper.ToSummaryDto(blog, langEnum)).ToList();
    }

    public async Task<BlogPostSummaryDto?> GetByIdAsync(Guid id, string lang)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return null;

        var langEnum = Enum.TryParse<LanguageCode>(lang, true, out var parsedLang) ? parsedLang : LanguageCode.En;
        return BlogPostMapper.ToSummaryDto(entity, langEnum);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto)
    {
        var entity = BlogPostMapper.FromCreateDto(dto);
        await _repository.AddAsync(entity);
        return BlogPostMapper.ToDto(entity);
    }

    public async Task UpdateAsync(Guid id, UpdateBlogPostDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new Exception("Blog post not found");

        BlogPostMapper.ApplyUpdateDto(entity, dto);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new Exception("Blog post not found");

        await _repository.DeleteAsync(entity);
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _repository.IncrementViewCountAsync(id);
    }
}
