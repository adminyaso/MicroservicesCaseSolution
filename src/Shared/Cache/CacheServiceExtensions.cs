using Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cache
{
    public static class CacheServiceExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration üzerinden redis bağlantısını alıyoruz
            // Config yoksa bağlanana kadar dener.
            var redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379,AbortOnConnectFail=false";
            // Redis connection thread-safe olduğundan Singleton olarak kayıt ediyoruz.
            services.AddSingleton<ICacheService>(new RedisCacheService(redisConnectionString));
            return services;
        }
    }
}
