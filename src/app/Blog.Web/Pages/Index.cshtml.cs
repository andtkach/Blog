using Blog.Web.Models.Domain;
using Blog.Web.Repositories;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IArticleRepository articleRepository;
        private readonly ITagRepository tagRepository;
        private readonly IArticleService articleService;

        public List<Article> Blogs { get; set; } = null!;
        public List<Tag> Tags { get; set; } = null!;

        public IndexModel(ILogger<IndexModel> logger,
            IArticleRepository articleRepository,
            ITagRepository tagRepository, IArticleService articleService)
        {
            this.logger = logger;
            this.articleRepository = articleRepository;
            this.tagRepository = tagRepository;
            this.articleService = articleService;
        }

        public async Task<IActionResult> OnGet()
        {
            Blogs = (await articleService.GetAllAsync()).ToList();
            Tags = (await tagRepository.GetAllAsync()).ToList();
            logger.LogInformation("Get all articles");
            return Page();
        }
    }
}