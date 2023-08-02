using System.Threading.Channels;

namespace Blog.Web.BackgroundServices
{
    public class ArticleCacheProcessingChannel
    {
        private const int MaxMessagesInChannel = 100;

        private readonly Channel<string> channel;
        private readonly ILogger<ArticleCacheProcessingChannel> logger;

        public ArticleCacheProcessingChannel(ILogger<ArticleCacheProcessingChannel> logger)
        {
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = false,
                SingleReader = true
            };

            this.channel = Channel.CreateBounded<string>(options);
            this.logger = logger;
        }

        public async Task<bool> ProcessArticleAsync(string articleId, CancellationToken ct = default)
        {
            while (await channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                if (channel.Writer.TryWrite(articleId))
                {
                    Log.ChannelMessageWritten(logger, articleId);
                    return true;
                }
            }

            return false;
        }

        public IAsyncEnumerable<string> ReadAllAsync(CancellationToken ct = default) =>
            channel.Reader.ReadAllAsync(ct);

        public bool TryCompleteWriter(Exception ex) => channel.Writer.TryComplete(ex);

        internal static class EventIds
        {
            public static readonly EventId ChannelMessageWritten = new EventId(100, "ChannelMessageWritten");
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> ChannelMessageWrittenAction = LoggerMessage.Define<string>(
                LogLevel.Information,
                EventIds.ChannelMessageWritten,
                "ArticleId {articleId} was written to the channel.");

            public static void ChannelMessageWritten(ILogger logger, string configurationId)
            {
                ChannelMessageWrittenAction(logger, configurationId, null!);
            }
        }
    }
}
