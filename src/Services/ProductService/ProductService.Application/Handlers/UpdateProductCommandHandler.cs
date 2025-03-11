using AutoMapper;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
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

            // Güncellenmiş ürünü ProductDto'ya mapleyip döndürüyoruz.
            var updatedProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
            return _mapper.Map<ProductDto>(updatedProduct);
        }
    }
}