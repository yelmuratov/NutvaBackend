    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutvaCms.Application.DTOs;
    using NutvaCms.Application.Interfaces;

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

        // Public access - fetch all products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() =>
            Ok(await _productService.GetAllAsync());

        // Public access - fetch a single product
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        // Authenticated admins only - create a product
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductDto dto)
        {
            var created = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Authenticated admins only - update a product
        [HttpPut("{id}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ProductDto dto)
        {
            var updated = await _productService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // Authenticated admins only - delete a product
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("view/{id}")]
        public async Task<IActionResult> TrackView(Guid id)
        {
            await _productService.IncrementViewAsync(id);
            return Ok(new { message = "Product view tracked." });
        }

        [HttpPost("buy-click/{id}")]
        public async Task<IActionResult> TrackBuyClick(Guid id)
        {
            await _productService.IncrementBuyClickAsync(id);
            return Ok(new { message = "Product buy click tracked." });
        }

    }
