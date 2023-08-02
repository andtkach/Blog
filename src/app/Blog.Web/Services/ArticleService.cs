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
        private readonly IContentCache contentCache;

        public ArticleService(IArticleRepository articleRepository, ILogger<ArticleService> logger,
            IContentCache contentCache)
        {
            this.articleRepository = articleRepository;
            this.logger = logger;
            this.contentCache = contentCache;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {

            if (contentCache.UseCache)
            {
                var cachedData = this.contentCache.Get<IEnumerable<Article>>(Constants.AllArticlesContentCachePrefix);
                if (cachedData != null)
                {
                    logger.LogInformation("Get articles from cache");
                    return cachedData;
                }
            }

            var result = await articleRepository.GetAllAsync();
            logger.LogInformation("Get articles from database");

            if (contentCache.UseCache)
            {
                this.contentCache.Add(Constants.AllArticlesContentCachePrefix, result, this.contentCache.ContentCacheSeconds);
            }

            return result;
        }

        public async Task<Article?> GetAsync(string urlHandle)
        {
            if (contentCache.UseCache)
            {
                var cachedData = this.contentCache.Get<Article>($"{Constants.OneArticleContentCachePrefix}_{urlHandle}");
                if (cachedData != null)
                {
                    logger.LogInformation("Get article from cache");
                    return cachedData;
                }
            }

            var result = await articleRepository.GetAsync(urlHandle);
            logger.LogInformation("Get article from database");

            if (contentCache.UseCache)
            {
                this.contentCache.Add($"{Constants.OneArticleContentCachePrefix}_{urlHandle}", result, this.contentCache.ContentCacheSeconds);
            }

            return result;
        }
    }
}
