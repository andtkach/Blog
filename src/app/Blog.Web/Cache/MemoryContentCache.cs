using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;

namespace Blog.Web.Cache
{
    public class MemoryContentCache : IContentCache
    {
        private readonly IMemoryCache memoryCache;

        public MemoryContentCache(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.ContentCacheSeconds = 60;
        }

        public bool UseCache { get; set; }
        public int ContentCacheSeconds { get; set; }

        public void Add<T>(string key, T item, int seconds = 60)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cache key could not be null");
            }

            var option = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds)
            };

            memoryCache.Set<T>(key, item, option);
        }

        public T? Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cache key could not be null");
            }

            var data = memoryCache.TryGetValue(key, out T? result) ? result : default(T);
            return data;
        }

        public void Remove(string key)
        {
            this.memoryCache.Remove(key);
        }
    }
}
