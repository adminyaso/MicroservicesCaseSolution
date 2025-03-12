using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Events;

namespace ProductService.Application.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Entity'den DTO'ya eşleme
            CreateMap<Product, ProductDto>();

            // DTO'dan Entity'ye eşleme
            CreateMap<CreateProductDto, Product>();

            CreateMap<UpdateProductDto, Product>();

            // Entity'den Publish'e eşleme
            CreateMap<Product, ProductCreatedEvent>();
            CreateMap<Product, ProductUpdatedEvent>();
        }
    }
}