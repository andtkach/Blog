using Blog.Web.Models.Domain;

namespace Blog.Web.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> AddAsync(Comment comment);

        Task<IEnumerable<Comment>> GetAllAsync(Guid articleId);
    }
}
