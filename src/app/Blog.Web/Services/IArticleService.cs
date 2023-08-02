using Blog.Web.Models.Domain;

namespace Blog.Web.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllAsync();
        Task<Article?> GetAsync(string urlHandle);
    }
}
