using NutvaCms.Application.DTOs.Blog;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers.Blog;
using NutvaCms.Domain.Enums;
using NutvaCms.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace NutvaCms.Application.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _repository;
    private readonly IFileService _fileService;

    private readonly string[] allowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
    private readonly string[] allowedVideoTypes = { "video/mp4" };

    public BlogPostService(IBlogPostRepository repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
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

        await ProcessMediaFiles(dto, entity);

        await _repository.AddAsync(entity);
        return BlogPostMapper.ToDto(entity);
    }

    public async Task UpdateAsync(Guid id, UpdateBlogPostDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new Exception("Blog post not found");

        BlogPostMapper.ApplyUpdateDto(entity, dto);

        await ProcessMediaFiles(dto, entity);

        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new Exception("Blog post not found");

        await _repository.DeleteAsync(entity);
    }

    private async Task ProcessMediaFiles(dynamic dto, BlogPost entity)
    {
        Console.WriteLine($"ImageFiles count: {((IEnumerable<IFormFile>)dto.ImageFiles)?.Count() ?? 0}");

        // ✅ Image files
        if (dto.ImageFiles != null)
        {
            foreach (var file in dto.ImageFiles)
            {
                if (file == null || file.Length == 0) continue;

                ValidateImageFile(file);
                var fileUrl = await _fileService.UploadSingleAsync(file);

                entity.Media.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = entity.Id,
                    Url = fileUrl,
                    MediaType = MediaType.Image
                });
            }
        }

        // ✅ Video files
        if (dto.VideoFiles != null)
        {
            foreach (var file in dto.VideoFiles)
            {
                if (file == null || file.Length == 0) continue;

                ValidateVideoFile(file);
                var fileUrl = await _fileService.UploadSingleAsync(file);

                entity.Media.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = entity.Id,
                    Url = fileUrl,
                    MediaType = MediaType.Video
                });
            }
        }

        // ✅ Image URLs
        if (dto.ImageUrls != null)
        {
            foreach (var url in ((IEnumerable<string>)dto.ImageUrls).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                ValidateImageUrl(url);
                entity.Media.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = entity.Id,
                    Url = url,
                    MediaType = MediaType.ImageUrl
                });
            }
        }

        // ✅ Video URLs
        if (dto.VideoUrls != null)
        {
            foreach (var url in ((IEnumerable<string>)dto.VideoUrls).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                ValidateVideoUrl(url);
                var mediaType = IsYoutubeUrl(url) ? MediaType.YoutubeUrl : MediaType.VideoUrl;
                entity.Media.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = entity.Id,
                    Url = url,
                    MediaType = mediaType
                });
            }
        }
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _repository.IncrementViewCountAsync(id);
    }


    // VALIDATIONS (Hardened & Exception Safe)

    private void ValidateImageFile(IFormFile file)
    {
        if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
            throw new Exception("Invalid image file type.");
    }

    private void ValidateVideoFile(IFormFile file)
    {
        if (!allowedVideoTypes.Contains(file.ContentType.ToLower()))
            throw new Exception("Invalid video file type.");
    }

    private void ValidateImageUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new Exception("Image URL cannot be empty.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new Exception("Invalid Image URL format.");

        var lower = url.ToLower();
        if (!(lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".png") || lower.EndsWith(".webp") || lower.EndsWith(".gif")))
            throw new Exception("Unsupported image URL extension.");
    }

    private void ValidateVideoUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new Exception("Video URL cannot be empty.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new Exception("Invalid Video URL format.");
    }
    private bool IsYoutubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        var youtubeRegex = new Regex(@"^(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+$");
        return youtubeRegex.IsMatch(url);
    }
}
