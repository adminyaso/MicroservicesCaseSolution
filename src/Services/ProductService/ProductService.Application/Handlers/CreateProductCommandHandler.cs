using AutoMapper;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateProductDto;
            var product = _mapper.Map<Product>(dto);
            await _productRepository.AddProductAsync(product);

            return _mapper.Map<ProductDto>(product);
        }
    }
}