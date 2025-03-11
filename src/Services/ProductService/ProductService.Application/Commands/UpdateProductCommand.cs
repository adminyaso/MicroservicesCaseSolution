using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands
{
    //[Authorize]
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public UpdateProductDto UpdateProductDto { get; }

        public UpdateProductCommand(UpdateProductDto dto)
        {
            UpdateProductDto = dto;
        }
    }
}