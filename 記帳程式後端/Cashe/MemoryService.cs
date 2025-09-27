using Microsoft.Extensions.Caching.Memory;
using 記帳程式後端.Contract.Cache;

namespace 記帳程式後端.Cashe
{
    public class MemoryService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task ClearAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string key)
        {
           return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            _memoryCache.TryGetValue(key, out T item);
            return Task.FromResult(item);
        }

        public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null) where T : class
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (expiration != null)
            {
                options.AbsoluteExpirationRelativeToNow = expiration; 
            }

            if (_memoryCache.TryGetValue(key, out T item))
            {
                return Task.FromResult(item);
            }
            item = getItem().GetAwaiter().GetResult();
            if (item != null)
            {
                _memoryCache.Set(key, item, options); // Fix: Pass the options object correctly
            }
            return Task.FromResult(item);
        }

        public Task<string?> GetStringAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (expiration != null)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task SetStringAsync(string key, string value, TimeSpan? expiration = null)
        {
            throw new NotImplementedException();
        }
    }
}
