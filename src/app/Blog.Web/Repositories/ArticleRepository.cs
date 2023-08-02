using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BlogDbContext blogDbContext;

        public ArticleRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        public async Task<Article> AddAsync(Article article)
        {
            await blogDbContext.Articles.AddAsync(article);
            await blogDbContext.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingBlog = await blogDbContext.Articles.FindAsync(id);

            if (existingBlog != null)
            {
                blogDbContext.Articles.Remove(existingBlog);
                await blogDbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await blogDbContext.Articles.Include(nameof(Article.Tags)).ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetAllAsync(string tagName)
        {
            return await (blogDbContext.Articles.Include(nameof(Article.Tags))
                .Where(x => x.Tags.Any(x => x.Name == tagName)))
                .ToListAsync();
        }

        public async Task<Article?> GetAsync(Guid id)
        {
            return await blogDbContext
                .Articles
                .Include(nameof(Article.Tags))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Article?> GetAsync(string urlHandle)
        {
            return await blogDbContext.Articles.Include(nameof(Article.Tags))
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<Article> UpdateAsync(Article article)
        {
            var existingArticle = await blogDbContext.Articles.Include(nameof(Article.Tags))
                .FirstOrDefaultAsync(x => x.Id == article.Id);

            if (existingArticle != null)
            {
                existingArticle.Heading = article.Heading;
                existingArticle.PageTitle = article.PageTitle;
                existingArticle.Content = article.Content;
                existingArticle.ShortDescription = article.ShortDescription;
                existingArticle.FeaturedImageUrl = article.FeaturedImageUrl;
                existingArticle.UrlHandle = article.UrlHandle;
                existingArticle.PublishedDate = article.PublishedDate;
                existingArticle.Author = article.Author;
                existingArticle.Visible = article.Visible;

                if (article.Tags.Any())
                {
                    // Delete the existing tags
                    blogDbContext.Tags.RemoveRange(existingArticle.Tags);

                    // Add new tags
                    article.Tags.ToList().ForEach(x => x.ArticleId = existingArticle.Id);
                    await blogDbContext.Tags.AddRangeAsync(article.Tags);
                }

                await blogDbContext.SaveChangesAsync();
                return existingArticle;
            }

            throw new ArgumentException("Unable to update article");
        }
    }
}
