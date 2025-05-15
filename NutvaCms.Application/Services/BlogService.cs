using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _repo;

    public BlogService(IBlogRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Blog>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Blog?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Blog> CreateAsync(BlogDto dto)
    {
        var blog = new Blog
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Slug,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            Images = dto.ImageUrls?.Select(url => new BlogImage { ImageUrl = url }).ToList() ?? new()
        };

        await _repo.AddAsync(blog);
        return blog;
    }

    public async Task<Blog?> UpdateAsync(Guid id, BlogDto dto)
    {
        var blog = await _repo.GetByIdAsync(id);
        if (blog == null) return null;

        blog.Title = dto.Title;
        blog.Content = dto.Content;
        blog.Slug = dto.Slug;
        blog.MetaTitle = dto.MetaTitle;
        blog.MetaDescription = dto.MetaDescription;
        blog.MetaKeywords = dto.MetaKeywords;
        blog.Images = dto.ImageUrls?.Select(url => new BlogImage { ImageUrl = url }).ToList() ?? new();

        await _repo.UpdateAsync(blog);
        return blog;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var blog = await _repo.GetByIdAsync(id);
        if (blog == null) return false;

        await _repo.DeleteAsync(blog);
        return true;
    }
}
