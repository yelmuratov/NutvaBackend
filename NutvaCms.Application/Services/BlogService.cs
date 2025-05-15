using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _repo;
    private readonly IFileService _fileService;

    public BlogService(IBlogRepository repo, IFileService fileService)
    {
        _repo = repo;
        _fileService = fileService;
    }

    public Task<IEnumerable<Blog>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Blog?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Blog> CreateAsync(BlogDto dto)
    {
        var urls = dto.Images != null && dto.Images.Any()
            ? await _fileService.UploadManyAsync(dto.Images)
            : new List<string>();

        var blog = new Blog
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Slug,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            ImageUrls = urls
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

        if (dto.Images != null && dto.Images.Any())
        {
            var urls = await _fileService.UploadManyAsync(dto.Images);
            blog.ImageUrls = urls;
        }

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

    public async Task IncrementViewAsync(Guid id)
    {
        await _repo.IncrementViewAsync(id);
    }

}
