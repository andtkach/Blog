using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext blogDbContext;

        public CommentRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            await blogDbContext.Comments.AddAsync(comment);
            await blogDbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync(Guid articleId)
        {
            return await blogDbContext.Comments.Where(x => x.ArticleId == articleId)
                .ToListAsync();
        }
    }
}
