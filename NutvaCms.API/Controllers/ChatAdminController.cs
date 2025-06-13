using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.Chat;
using NutvaCms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace NutvaCms.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatAdminController : ControllerBase
    {
        private readonly IChatAdminService _chatAdminService;

        public ChatAdminController(IChatAdminService chatAdminService)
        {
            _chatAdminService = chatAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _chatAdminService.GetAllAsync();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admin = await _chatAdminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound();
            return Ok(admin);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChatAdminDto dto)
        {
            var admin = await _chatAdminService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChatAdminDto dto)
        {
            var admin = await _chatAdminService.UpdateAsync(id, dto);
            if (admin == null)
                return NotFound();
            return Ok(admin);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _chatAdminService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
