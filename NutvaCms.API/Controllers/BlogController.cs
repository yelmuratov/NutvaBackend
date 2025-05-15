using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    // Public access - anyone can view all blogs
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() =>
        Ok(await _blogService.GetAllAsync());

    // Public access - anyone can view one blog
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _blogService.GetByIdAsync(id);
        return blog is null ? NotFound() : Ok(blog);
    }

    // Only logged-in admins can create blogs
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] BlogDto dto)
    {
        var created = await _blogService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // Only logged-in admins can update blogs
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] BlogDto dto)
    {
        var updated = await _blogService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // Only logged-in admins can delete blogs
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _blogService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("view/{id}")]
    public async Task<IActionResult> TrackView(Guid id)
    {
        await _blogService.IncrementViewAsync(id);
        return Ok(new { message = "Blog view tracked." });
    }

}
