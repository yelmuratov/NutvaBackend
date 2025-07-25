using Microsoft.AspNetCore.Http;
using NutvaCms.Application.DTOs.BlogMedia;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Domain.Enums;

namespace NutvaCms.Application.Services;

public class BlogMediaService : IBlogMediaService
{
    private readonly IBlogMediaRepository _mediaRepository;
    private readonly IFileService _fileService;

    public BlogMediaService(IBlogMediaRepository mediaRepository, IFileService fileService)
    {
        _mediaRepository = mediaRepository;
        _fileService = fileService;
    }

    public async Task<List<BlogMediaResponseDto>> CreateAsync(CreateBlogMediaDto dto)
    {
        var mediaList = new List<BlogPostMedia>();

        // Handle image files
        if (dto.ImageFiles != null)
        {
            foreach (var file in dto.ImageFiles)
            {
                if (file == null || file.Length == 0) continue;
                var url = await _fileService.UploadSingleAsync(file);

                mediaList.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = dto.BlogPostId,
                    Url = url,
                    MediaType = MediaType.Image
                });
            }
        }

        // Handle video files
        if (dto.VideoFiles != null)
        {
            foreach (var file in dto.VideoFiles)
            {
                if (file == null || file.Length == 0) continue;
                var url = await _fileService.UploadSingleAsync(file);

                mediaList.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = dto.BlogPostId,
                    Url = url,
                    MediaType = MediaType.Video
                });
            }
        }

        // Handle image URLs
        if (dto.ImageUrls != null)
        {
            foreach (var url in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                mediaList.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = dto.BlogPostId,
                    Url = url,
                    MediaType = MediaType.ImageUrl
                });
            }
        }

        // Handle video URLs
        if (dto.VideoUrls != null)
        {
            foreach (var url in dto.VideoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                var mediaType = IsYoutubeUrl(url) ? MediaType.YoutubeUrl : MediaType.VideoUrl;
                mediaList.Add(new BlogPostMedia
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = dto.BlogPostId,
                    Url = url,
                    MediaType = mediaType
                });
            }
        }

        await _mediaRepository.AddManyAsync(mediaList);

        return mediaList.Select(m => new BlogMediaResponseDto
        {
            Id = m.Id,
            Url = m.Url,
            Caption = m.Caption,
            AltText = m.AltText,
            MediaType = m.MediaType
        }).ToList();
    }

    public async Task UpdateAsync(List<UpdateBlogMediaDto> dtoList)
    {
        var ids = dtoList.Select(d => d.Id).ToList();
        var existing = await _mediaRepository.GetByIdsAsync(ids);

        foreach (var dto in dtoList)
        {
            var media = existing.FirstOrDefault(m => m.Id == dto.Id);
            if (media == null) continue;

            media.Caption = dto.Caption;
            media.AltText = dto.AltText;
        }

        await _mediaRepository.UpdateManyAsync(existing);
    }

    public async Task DeleteAsync(List<Guid> ids)
    {
        await _mediaRepository.DeleteManyAsync(ids);
    }

    public async Task<BlogMediaResponseDto?> GetByIdAsync(Guid id)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        if (media == null) return null;

        return new BlogMediaResponseDto
        {
            Id = media.Id,
            Url = media.Url,
            Caption = media.Caption,
            AltText = media.AltText,
            MediaType = media.MediaType
        };
    }

    public async Task<List<BlogMediaResponseDto>> GetByPostIdAsync(Guid postId)
    {
        var mediaList = await _mediaRepository.GetByPostIdAsync(postId);

        return mediaList.Select(m => new BlogMediaResponseDto
        {
            Id = m.Id,
            Url = m.Url,
            Caption = m.Caption,
            AltText = m.AltText,
            MediaType = m.MediaType
        }).ToList();
    }

    private bool IsYoutubeUrl(string url)
    {
        return url.Contains("youtube.com") || url.Contains("youtu.be");
    }
}
