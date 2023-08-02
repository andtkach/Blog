using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BlogDbContext blogDbContext;

        public TagRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }


        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            var tags = await blogDbContext.Tags.ToListAsync();

            return tags.DistinctBy(x => x.Name.ToLower());
        }
    }
}
