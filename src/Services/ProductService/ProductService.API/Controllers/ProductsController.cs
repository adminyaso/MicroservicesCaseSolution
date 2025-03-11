using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Ürün ekleme
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var command = new CreateProductCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Ürün güncelleme
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var command = new UpdateProductCommand(dto);
            var result = await _mediator.Send(command);
            if (result == null)
            {
                return NotFound("Ürün bulunamadı.");
            }
            return Ok(result);
        }

        // Ürün listeleme
        [HttpGet("List")]
        public async Task<IActionResult> ListProducts()
        {
            var query = new GetAllProductsQuery();
            IEnumerable<ProductDto> products = await _mediator.Send(query);
            return Ok(products);
        }
    }
}