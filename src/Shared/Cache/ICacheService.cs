using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
