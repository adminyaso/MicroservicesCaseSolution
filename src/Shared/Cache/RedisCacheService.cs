
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cache
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;
            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public void Dispose()
        {
            if (_redis != null)
            {
                _redis.Close();
                _redis.Dispose();
            }
        }
    }
}
