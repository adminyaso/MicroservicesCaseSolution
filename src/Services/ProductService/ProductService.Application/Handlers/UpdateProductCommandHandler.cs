using AutoMapper;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Events;

namespace ProductService.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var updateDto = request.UpdateProductDto;

            // Mevcut ürünü repository üzerinden çekiyoruz.
            var product = await _productRepository.GetProductByIdAsync(updateDto.ProductId);
            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            // AutoMapper kullanarak updateDto'dan gelen verilerle mevcut ürünü güncelliyoruz.
            _mapper.Map(updateDto, product);

            // Repository aracılığıyla güncellenen ürünü kaydediyoruz.
            await _productRepository.UpdateProductAsync(product);
            // Ürün güncelleme işlemi başarılıysa, event oluştur ve yayınla
            var productUpdatedEvent = _mapper.Map<ProductUpdatedEvent>(product);
            // UpdateEvent'i asenkron olarak Kafka üzerinden yayınla
            await _eventPublisher.PublishAsync(productUpdatedEvent, cancellationToken);
            // Güncellenmiş ürünü ProductDto'ya mapleyip döndürüyoruz.
            var updatedProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
            return _mapper.Map<ProductDto>(updatedProduct);
        }
    }
}