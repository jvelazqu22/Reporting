using System;
using System.Runtime.Caching;
using Domain.Helper;

namespace Domain.Services
{
    public interface ICacheService
    {
        /// <summary>
        /// Attempts to retrieve an item from the cache using <paramref name="key"/>. If a value exists it is assigned to <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Success or failure retrieving value from cache</returns>
        bool TryGetValue<T>(CacheKeys key, out T value);

        /// <summary>
        /// Adds <paramref name="value"/> to the cache, keyed on <paramref name="key"/>, with an expiration of <paramref name="absoluteExpiration"/>. If a value already exists 
        /// at that key the add will fail.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        void Add(CacheKeys key, object value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Adds <paramref name="value"/> to the cache, keyed on <paramref name="key"/>, with an expiration of <paramref name="absoluteExpiration"/>. If a value already exists 
        /// at that key it will be overwritten.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        void Set(CacheKeys key, object value, DateTimeOffset absoluteExpiration);
    }

    public class CacheService : ICacheService
    {
        private readonly ObjectCache _cache;

        public CacheService()
        {
            _cache = MemoryCache.Default;
        }

        /// <summary>
        /// Attempts to retrieve an item from the cache using <paramref name="key"/>. If a value exists it is assigned to <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Success or failure retrieving value from cache</returns>
        public bool TryGetValue<T>(CacheKeys key, out T value)
        {
            var temp = _cache.Get(key.ToString());

            if (temp == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = (T)temp;
                return true;
            }
        }

        public void Add(CacheKeys key, object value, DateTimeOffset absoluteExpiration)
        {
            _cache.Add(key.ToString(), value, absoluteExpiration);
        }

        /// <summary>
        /// Adds <paramref name="value"/> to the cache, keyed on <paramref name="key"/>, with an expiration of <paramref name="absoluteExpiration"/>. If a value already exists 
        /// at that key it will be overwritten.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        public void Set(CacheKeys key, object value, DateTimeOffset absoluteExpiration)
        {
            _cache.Set(key.ToString(), value, absoluteExpiration);
        }
    }
}
