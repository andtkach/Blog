using System.Text.Json;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Admin.Blogs
{
    [Authorize(Roles = "Moderator, Administrator")]
    public class ListModel : PageModel
    {
        private readonly IArticleRepository articleRepository;

        public List<Article> Articles { get; set; } = null!;

        public ListModel(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public async Task OnGet()
        {
            var tempData = TempData["Notification"];
            if (tempData != null)
            {
                var notificationJson = (string)tempData;
                ViewData["Notification"] = JsonSerializer.Deserialize<Notification>(notificationJson);
            }

            Articles = new List<Article>();

            var list = await articleRepository.GetAllAsync();
            var articles = list.ToList();
            
            if (articles.Any())
            {
                Articles = articles;
            }
        }
    }
}
