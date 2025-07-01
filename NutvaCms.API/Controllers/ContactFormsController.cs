using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

[ApiController]
[Route("api/contact-forms")]
public class ContactFormsController : ControllerBase
{
    private readonly IContactFormService _service;
    public ContactFormsController(IContactFormService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactFormDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Phone))
            return BadRequest(new { message = "Name and phone are required." });

        var success = await _service.SubmitContactFormAsync(dto);
        if (!success)
            return StatusCode(500, new { message = "Failed to submit contact form." });

        return Ok(new { message = "Contact form submitted" });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }
}
