namespace Blog.Web.Cache
{
    public interface IArticleCache
    {
        bool UseCache { get; set; }
        int CacheSeconds { get; set; }
        void Add<T>(string key, T value, int minutes = 10);
        T? Get<T>(string key);
        void Remove(string key);
    }
}
