using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Blog.Web.BackgroundServices;
using Blog.Web.Enums;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Admin.Blogs
{
    [Authorize(Roles = "Moderator, Administrator")]
    public class AddModel : PageModel
    {
        private readonly IArticleRepository articleRepository;
        private readonly ArticleCacheProcessingChannel _articleCacheProcessingChannel;

        [BindProperty]
        public AddArticle AddArticleRequest { get; set; } = null!;

        [BindProperty]
        [Required]
        public string Tags { get; set; } = null!;

        public AddModel(IArticleRepository articleRepository, ArticleCacheProcessingChannel articleCacheProcessingChannel)
        {
            this.articleRepository = articleRepository;
            this._articleCacheProcessingChannel = articleCacheProcessingChannel;
        }

        public void OnGet()
        {
            //// Empty page
        }

        public async Task<IActionResult> OnPost()
        {
            ValidateAddArticle();

            if (ModelState.IsValid)
            {
                var article = new Article()
                {
                    Heading = AddArticleRequest.Heading,
                    PageTitle = AddArticleRequest.PageTitle,
                    Content = AddArticleRequest.Content,
                    ShortDescription = AddArticleRequest.ShortDescription,
                    FeaturedImageUrl = AddArticleRequest.FeaturedImageUrl,
                    UrlHandle = AddArticleRequest.UrlHandle,
                    PublishedDate = AddArticleRequest.PublishedDate,
                    Author = AddArticleRequest.Author,
                    Visible = AddArticleRequest.Visible,
                    Tags = new List<Tag>(Tags.Split(',').Select(x => new Tag() { Name = x.Trim() }))
                };

                await articleRepository.AddAsync(article);

                await this._articleCacheProcessingChannel.ProcessArticleAsync(article.Id.ToString());

                var notification = new Notification
                {
                    Type = NotificationType.Success,
                    Message = "New blog created!"
                };

                TempData["Notification"] = JsonSerializer.Serialize(notification);

                return RedirectToPage("/Admin/Blogs/List");
            }

            return Page();
        }

        private void ValidateAddArticle()
        {
            if (AddArticleRequest.PublishedDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("AddArticleRequest.PublishedDate",
                    $"PublishedDate can only be today's date or a future date.");
            }
        }
    }
}
