using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces
{
    public interface IProductRepository
    {
        //Repository pattern : query handlers işlemleri için
        //Infrastructure katmanından veri çekme(Db)
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product> GetProductByIdAsync(long id);

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
    }
}