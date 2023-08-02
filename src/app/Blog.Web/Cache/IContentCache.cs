namespace Blog.Web.Cache
{
    public interface IContentCache
    {
        bool UseCache { get; set; }
        int ContentCacheSeconds { get; set; }
        void Add<T>(string key, T value, int minutes = 10);
        T? Get<T>(string key);
        void Remove(string key);
    }
}
