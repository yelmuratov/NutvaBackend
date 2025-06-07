using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.Blog;
using NutvaCms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace NutvaCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogPostController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string lang = "en")
    {
        var blogs = await _blogPostService.GetAllAsync(lang);
        return Ok(blogs);
    }


    /// <summary>
    /// Get blog post by Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _blogPostService.GetByIdAsync(id);
        if (blog == null)
            return NotFound();

        return Ok(blog);
    }

    /// <summary>
    /// Create new blog post (multipart/form-data for file uploads)
    /// </summary>
    [HttpPost]
    [Authorize] // Assuming you want to restrict this to authenticated users
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateBlogPostDto dto)
    {
        var created = await _blogPostService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update existing blog post (multipart/form-data for file uploads)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize] // Assuming you want to restrict this to authenticated users
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateBlogPostDto dto)
    {
        await _blogPostService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Delete blog post by Id
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize] // Assuming you want to restrict this to authenticated users
    public async Task<IActionResult> Delete(Guid id)
    {
        await _blogPostService.DeleteAsync(id);
        return NoContent();
    }
}
