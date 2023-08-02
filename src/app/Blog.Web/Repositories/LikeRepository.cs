using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly BlogDbContext blogDbContext;

        public LikeRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        public async Task AddLikeForBlog(Guid articleId, Guid userId)
        {
            var like = new Like
            {
                Id = Guid.NewGuid(),
                ArticleId = articleId,
                UserId = userId
            };

            await blogDbContext.Likes.AddAsync(like);
            await blogDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Like>> GetLikesForBlog(Guid articleId)
        {
            return await blogDbContext.Likes.Where(x => x.ArticleId == articleId)
                .ToListAsync();
        }

        public async Task<int> GetTotalLikesForBlog(Guid articleId)
        {
            return await blogDbContext.Likes
                .CountAsync(x => x.ArticleId == articleId);
        }
    }
}
