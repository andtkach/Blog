using Azure.Core;
using Blog.Web.Cache;
using Blog.Web.Models.Domain;
using Blog.Web.Repositories;
using Microsoft.VisualBasic;

namespace Blog.Web.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository articleRepository;
        private readonly ILogger<ArticleService> logger;
        private readonly IArticleCache articleCache;

        public ArticleService(IArticleRepository articleRepository, ILogger<ArticleService> logger,
            IArticleCache articleCache)
        {
            this.articleRepository = articleRepository;
            this.logger = logger;
            this.articleCache = articleCache;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {

            if (articleCache.UseCache)
            {
                var cachedData = this.articleCache.Get<IEnumerable<Article>>(Constants.AllArticlesCachePrefix);
                if (cachedData != null)
                {
                    logger.LogInformation("Get articles from cache");
                    return cachedData;
                }
            }

            var result = await articleRepository.GetAllAsync();
            logger.LogInformation("Get articles from database");

            if (articleCache.UseCache)
            {
                this.articleCache.Add(Constants.AllArticlesCachePrefix, result, this.articleCache.CacheSeconds);
            }

            return result;
        }

        public async Task<Article?> GetAsync(string urlHandle)
        {
            if (articleCache.UseCache)
            {
                var cachedData = this.articleCache.Get<Article>($"{Constants.OneArticleCachePrefix}_{urlHandle}");
                if (cachedData != null)
                {
                    logger.LogInformation("Get article from cache");
                    return cachedData;
                }
            }

            var result = await articleRepository.GetAsync(urlHandle);
            logger.LogInformation("Get article from database");

            if (articleCache.UseCache)
            {
                this.articleCache.Add($"{Constants.OneArticleCachePrefix}_{urlHandle}", result, this.articleCache.CacheSeconds);
            }

            return result;
        }
    }
}
