using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public CreateProductDto CreateProductDto { get; }

        public CreateProductCommand(CreateProductDto dto)
        {
            CreateProductDto = dto;
        }
    }
}