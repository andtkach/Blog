using Blog.Web.Cache;
using Blog.Web.Repositories;
using Microsoft.VisualBasic;

namespace Blog.Web.BackgroundServices
{
    public class RequestCacheService : BackgroundService
    {
        private readonly ILogger<RequestCacheService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ArticleCacheProcessingChannel _contentCacheProcessingChannel;

        public RequestCacheService(ILogger<RequestCacheService> logger, IServiceProvider serviceProvider,
            ArticleCacheProcessingChannel contentCacheProcessingChannel)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._contentCacheProcessingChannel = contentCacheProcessingChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"RequestCacheService started");

            await foreach (var articleId in this._contentCacheProcessingChannel.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation($"Delivered {articleId} for cache invalidation");

                    using var scope = _serviceProvider.CreateScope();

                    var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<IContentCache>();
                    
                    if (!cacheService.UseCache)
                    {
                        _logger.LogInformation("API configured to not use cache, so exit. ContentCache:UseCache");
                        return;
                    }

                    var allArticles = (await articleRepository.GetAllAsync()).ToList();
                    _logger.LogInformation($"Found {allArticles.Count()} articles");

                    foreach (var article in allArticles)
                    {
                        string key = $"{Constants.OneArticleContentCachePrefix}_{article.UrlHandle}";
                        cacheService.Add(key, article, cacheService.ContentCacheSeconds);
                        _logger.LogInformation($"Cached article: {article.Id} with a key {key}");
                    }

                    if (allArticles.Any())
                    {
                        cacheService.Add(Constants.AllArticlesContentCachePrefix, allArticles, cacheService.ContentCacheSeconds);
                        _logger.LogInformation("All articles are cached.");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error while executing RequestCacheService. {e}", e);
                }
            }
        }
    }
}
