
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Shared.Cache
{
    public class RedisCacheService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(string connectionString)
        {
            // Redis bağlantısını kuruyoruz.
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        /// Belirtilen anahtara verilen değeri cache'e yazar.
        public async Task SetAsync(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }

        /// Belirtilen anahtara karşılık gelen değeri cache'den alır.
        public async Task<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        /// Belirtilen anahtarın cache'den silinmesini sağlar.
        public async Task<bool> RemoveAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }
    }
}
