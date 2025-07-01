using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductBoxPriceController : ControllerBase
    {
        private readonly IProductBoxPriceService _service;
        public ProductBoxPriceController(IProductBoxPriceService service)
        {
            _service = service;
        }

        // GET: api/ProductBoxPrice/by-product/{productId}
        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<List<ProductBoxPriceDto>>> GetByProduct(Guid productId)
            => Ok(await _service.GetByProductAsync(productId));

        // GET: api/ProductBoxPrice/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductBoxPriceDto>> Get(Guid id)
        {
            var item = await _service.GetAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // GET: api/ProductBoxPrice/by-product-and-box?productId={productId}&boxCount={boxCount}
        [HttpGet("by-product-and-box")]
        public async Task<ActionResult<ProductBoxPriceDto>> GetByProductAndBox([FromQuery] Guid productId, [FromQuery] int boxCount)
        {
            var item = await _service.GetByProductAndBoxCountAsync(productId, boxCount);
            return item == null ? NotFound() : Ok(item);
        }

        // POST: api/ProductBoxPrice
        [HttpPost]
        public async Task<ActionResult<ProductBoxPriceDto>> Create([FromBody] CreateProductBoxPriceDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // PUT: api/ProductBoxPrice/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductBoxPriceDto>> Update(Guid id, [FromBody] UpdateProductBoxPriceDto dto)
            => Ok(await _service.UpdateAsync(id, dto));

        // DELETE: api/ProductBoxPrice/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
