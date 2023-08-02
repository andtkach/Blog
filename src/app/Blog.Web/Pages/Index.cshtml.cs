using Blog.Web.Models.Domain;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IArticleRepository articleRepository;
        private readonly ITagRepository tagRepository;

        public List<Article> Blogs { get; set; } = null!;
        public List<Tag> Tags { get; set; } = null!;

        public IndexModel(ILogger<IndexModel> logger,
            IArticleRepository articleRepository,
            ITagRepository tagRepository)
        {
            this.logger = logger;
            this.articleRepository = articleRepository;
            this.tagRepository = tagRepository;
        }

        public async Task<IActionResult> OnGet()
        {
            Blogs = (await articleRepository.GetAllAsync()).ToList();
            Tags = (await tagRepository.GetAllAsync()).ToList();
            logger.LogInformation("Get all articles");
            return Page();
        }
    }
}