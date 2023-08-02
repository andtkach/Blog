using Blog.Web.Models.Domain;

namespace Blog.Web.Repositories
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync();
        Task<IEnumerable<Article>> GetAllAsync(string tagName);
        Task<Article?> GetAsync(Guid id);
        Task<Article?> GetAsync(string urlHandle);
        Task<Article> AddAsync(Article article);
        Task<Article> UpdateAsync(Article article);
        Task<bool> DeleteAsync(Guid id);
    }
}
