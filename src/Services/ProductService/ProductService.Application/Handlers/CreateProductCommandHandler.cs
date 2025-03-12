using AutoMapper;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Events;

namespace ProductService.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateProductDto;
            var product = _mapper.Map<Product>(dto);
            await _productRepository.AddProductAsync(product);
            // Event oluştur
            var productCreatedEvent = _mapper.Map<ProductCreatedEvent>(product);
            // Event'i asenkron olarak Kafka üzerinden yayınla
            await _eventPublisher.PublishAsync(productCreatedEvent, cancellationToken);
            return _mapper.Map<ProductDto>(product);
        }
    }
}