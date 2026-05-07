using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    /// <summary>
    /// Interface for distributed caching (Redis)
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets data from cache
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Sets data in cache with a sliding expiration
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Removes data from cache by exact key
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Removes data from cache by prefix (useful for clearing all search results)
        /// </summary>
        Task RemoveByPrefixAsync(string prefixKey);
    }
}
