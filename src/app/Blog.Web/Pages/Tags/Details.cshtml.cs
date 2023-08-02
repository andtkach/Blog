using Blog.Web.Models.Domain;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Tags
{
    public class DetailsModel : PageModel
    {
        private readonly IArticleRepository articleRepository;

        public List<Article>? Blogs { get; set; }

        public DetailsModel(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public async Task<IActionResult> OnGet(string tagName)
        {
            Blogs = (await articleRepository.GetAllAsync(tagName)).ToList();
            return Page();
        }
    }
}
