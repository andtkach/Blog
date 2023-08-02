using Blog.Web.Cache;
using Blog.Web.Repositories;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace Blog.Web.BackgroundServices
{
    public class TimeCacheService : BackgroundService
    {
        private readonly ILogger<TimeCacheService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TimeCacheService(ILogger<TimeCacheService> logger, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TimeCacheService started");

            int refreshIntervalSeconds = 60;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var dt = DateTime.UtcNow;
                    _logger.LogInformation($"TimeCacheService populating cache at {dt}", dt);

                    using var scope = _serviceProvider.CreateScope();

                    var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<IContentCache>();
                    
                    if (!cacheService.UseCache)
                    {
                        _logger.LogInformation("API configured to not use cache, so exit. ContentCache:UseCache");
                        return;
                    }

                    refreshIntervalSeconds = cacheService.ContentCacheSeconds;

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
                    _logger.LogError($"Error while executing TimeCacheService. {e}", e);
                }

                int timeToSleep = refreshIntervalSeconds > 10 ? refreshIntervalSeconds - 9 : refreshIntervalSeconds;
                _logger.LogInformation($"TimeCacheService sleep for {timeToSleep} seconds", timeToSleep);
                await Task.Delay(TimeSpan.FromSeconds(timeToSleep), stoppingToken);
            }
        }
    }
}
