using Blog.Web.Models.Domain;

namespace Blog.Web.Repositories
{
    public interface ILikeRepository
    {
        Task<int> GetTotalLikesForBlog(Guid articleId);

        Task AddLikeForBlog(Guid articleId, Guid userId);

        Task<IEnumerable<Like>> GetLikesForBlog(Guid articleId);
    }
}
