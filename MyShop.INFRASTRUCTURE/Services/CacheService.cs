using Microsoft.Extensions.Caching.Distributed;
using MyShop.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;

        public CacheService(IDistributedCache cache, IConnectionMultiplexer redis)
        {
            _cache = cache;
            _redis = redis;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            string? cachedValue = await _cache.GetStringAsync(key);

            if (cachedValue is null)
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };
            string serializedValue = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedValue, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefixKey)
        {
            foreach (var endpoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endpoint);
                var keys = server.Keys(pattern: $"{prefixKey}*");

                foreach (var key in keys)
                {
                    await _redis.GetDatabase().KeyDeleteAsync(key);
                }
            }
        }
    }
}
