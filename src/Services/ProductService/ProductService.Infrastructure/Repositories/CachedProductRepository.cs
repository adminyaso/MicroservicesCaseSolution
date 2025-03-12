using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using Cache; //ICacheService için

namespace ProductService.Infrastructure.Repositories
{
    // IProductRepository'yi saran, decorator design patterni.
    // Cache Aside Pattern'i uygulayarak, okuma işlemlerinde cache’i kontrol eder
    /// ve yazma işlemleri sonrası cache invalidation yapar.
    public class CachedProductRepository : IProductRepository
    {
        private readonly IProductRepository _innerRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKey = "ProductList"; // Cache anahtarı
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5); // Cache süresi

        public CachedProductRepository(IProductRepository innerRepository, ICacheService cacheService)
        {
            _innerRepository = innerRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // Cache'te ürün listesi var mı
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<Product>>(CacheKey);
            if (cachedProducts != null)
            {
                // Cache'de veri varsa, direkt onu döner.
                return cachedProducts;
            }

            // Cache'de yoksa, veritabanından ürünleri çekiyoruz.
            var products = await _innerRepository.GetAllProductsAsync();

            // Çekilen veriyi Redis cache'e belirlenen süreyle kaydetme.
            await _cacheService.SetAsync(CacheKey, products, CacheDuration);

            return products;
        }

        public async Task<Product> GetProductByIdAsync(long id)
        {
            var cacheKey = $"Product_{id}";
            var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            var product = await _innerRepository.GetProductByIdAsync(id);
            if (product != null)
            {
                await _cacheService.SetAsync(cacheKey, product, CacheDuration);
            }
            return product;
        }

        // Ürün ekleme işlemi sonrası cache.
        public async Task AddProductAsync(Product product)
        {
            await _innerRepository.AddProductAsync(product);
            // Ürün cache'ini de güncelleme.
            var cacheKey = $"Product_{product.ProductId}";
            await _cacheService.SetAsync(cacheKey, product, CacheDuration);
        }

        // Ürün güncelleme işlemi sonrası cache
        public async Task UpdateProductAsync(Product product)
        {
            await _innerRepository.UpdateProductAsync(product);
            // Ürün güncelleme sonrası cache invalidation
            //await _cacheService.RemoveAsync(CacheKey);
            var cacheKey = $"Product_{product.ProductId}";
            await _cacheService.RemoveAsync(cacheKey);
            // Opsiyonel: Yeni veriyi cache'e ekleyebilirsiniz.
            await _cacheService.SetAsync(cacheKey, product, CacheDuration);
        }
    }
}
