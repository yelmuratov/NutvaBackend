using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutvaCms.Application.DTOs.ProductDtos;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Mappers;
using NutvaCms.Domain.Enums;

namespace NutvaCms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // ✅ Public access - fetch all products with language param
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string lang = "en")
    {
        var products = await _productService.GetAllAsync(lang);
        return Ok(products);
    }

    // ✅ Public access - fetch full product entity by id
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] string lang = "en")
    {
        var productDto = await _productService.GetByIdAsync(id, lang);
        return productDto is null ? NotFound() : Ok(productDto);
    }

    // ✅ Authenticated admins only - create product
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
    {
        var created = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ✅ Authenticated admins only - update product
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // ✅ Authenticated admins only - delete product
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("view/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackView(Guid id)
    {
        await _productService.IncrementViewAsync(id);
        return Ok(new { message = "Product view tracked." });
    }

    [HttpPost("buy-click/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackBuyClick(Guid id)
    {
        await _productService.IncrementBuyClickAsync(id);
        return Ok(new { message = "Product buy click tracked." });
    }
}
