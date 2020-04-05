using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class BaseRepository
    {
        private readonly MemoryCache cache;

        protected BaseRepository()
        {
            this.cache = new MemoryCache(new MemoryCacheOptions());
        }

        public Task Set<T>(string id, T data)
        {
            var cacheEntry = new MemoryCacheEntryOptions();
            cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            cache.Set(id, data, cacheEntry);

            return Task.CompletedTask;
        }

        public Task<T> Get<T>(string id)
        {
            var result = cache.Get<T>(id);

            return Task.FromResult(result);
        }

        public Task Remove(string id)
        {
            cache.Remove(id);

            return Task.CompletedTask;
        }
    }
}