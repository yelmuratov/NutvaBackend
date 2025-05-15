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

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _blogService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _blogService.GetByIdAsync(id);
        return blog is null ? NotFound() : Ok(blog);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogDto dto)
    {
        var created = await _blogService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BlogDto dto)
    {
        var updated = await _blogService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _blogService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
