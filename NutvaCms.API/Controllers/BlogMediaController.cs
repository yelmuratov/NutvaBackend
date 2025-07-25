using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NutvaCms.Application.DTOs.BlogMedia;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/blog-media")]
[Authorize] // ✅ Only authorized users can access
public class BlogMediaController : ControllerBase
{
    private readonly IBlogMediaService _blogMediaService;

    public BlogMediaController(IBlogMediaService blogMediaService)
    {
        _blogMediaService = blogMediaService;
    }

    // POST: /api/blog-media
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreateMedia([FromForm] CreateBlogMediaDto dto)
    {
        var result = await _blogMediaService.CreateAsync(dto);
        return Ok(new { success = true, data = result });
    }

    // PUT: /api/blog-media
    [HttpPut]
    public async Task<IActionResult> UpdateMedia([FromBody] List<UpdateBlogMediaDto> dtoList)
    {
        await _blogMediaService.UpdateAsync(dtoList);
        return Ok(new { success = true, message = "Media updated successfully" });
    }

    // DELETE: /api/blog-media
    [HttpDelete]
    public async Task<IActionResult> DeleteMedia([FromBody] DeleteBlogMediaDto dto)
    {
        await _blogMediaService.DeleteAsync(dto.Ids);
        return Ok(new { success = true, message = "Media deleted successfully" });
    }

    // GET: /api/blog-media/post/{postId}
    [HttpGet("post/{postId:guid}")]
    public async Task<IActionResult> GetMediaByPostId(Guid postId)
    {
        var result = await _blogMediaService.GetByPostIdAsync(postId);
        return Ok(new { success = true, data = result });
    }

    // GET: /api/blog-media/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMediaById(Guid id)
    {
        var result = await _blogMediaService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { success = false, message = "Media not found" });

        return Ok(new { success = true, data = result });
    }
}
