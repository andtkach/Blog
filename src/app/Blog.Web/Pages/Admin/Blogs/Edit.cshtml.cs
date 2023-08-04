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
    public class EditModel : PageModel
    {
        private readonly IArticleRepository articleRepository;
        private readonly ILogger<EditModel> logger;
        private readonly ArticleCacheProcessingChannel _articleCacheProcessingChannel;

        [BindProperty]
        public EditArticleRequest ArticleRequest { get; set; } = null!;

        [BindProperty]
        [Required]
        public string Tags { get; set; } = null!;

        public EditModel(IArticleRepository articleRepository, ILogger<EditModel> logger,
            ArticleCacheProcessingChannel articleCacheProcessingChannel)
        {
            this.articleRepository = articleRepository;
            this.logger = logger;
            this._articleCacheProcessingChannel = articleCacheProcessingChannel;
        }

        public async Task OnGet(Guid id)
        {
            var article = await articleRepository.GetAsync(id);

            if (article != null && article.Tags != null)
            {
                ArticleRequest = new EditArticleRequest
                {
                    Id = article.Id,
                    Heading = article.Heading,
                    PageTitle = article.PageTitle,
                    Content = article.Content,
                    ShortDescription = article.ShortDescription,
                    FeaturedImageUrl = article.FeaturedImageUrl,
                    UrlHandle = article.UrlHandle,
                    PublishedDate = article.PublishedDate,
                    Author = article.Author,
                    Visible = article.Visible
                };

                Tags = string.Join(',', article.Tags.Select(x => x.Name));
            }
        }

        public async Task<IActionResult> OnPostEdit()
        {
            ValidateEditArticle();

            if (ModelState.IsValid)
            {
                try
                {
                    var article = new Article()
                    {
                        Id = ArticleRequest.Id,
                        Heading = ArticleRequest.Heading,
                        PageTitle = ArticleRequest.PageTitle,
                        Content = ArticleRequest.Content,
                        ShortDescription = ArticleRequest.ShortDescription,
                        FeaturedImageUrl = ArticleRequest.FeaturedImageUrl,
                        UrlHandle = ArticleRequest.UrlHandle,
                        PublishedDate = ArticleRequest.PublishedDate,
                        Author = ArticleRequest.Author,
                        Visible = ArticleRequest.Visible,
                        Tags = new List<Tag>(Tags.Split(',').Select(x => new Tag() { Name = x.Trim() }))
                    };


                    await articleRepository.UpdateAsync(article);
                    await this._articleCacheProcessingChannel.ProcessArticleAsync(article.Id.ToString());

                    ViewData["Notification"] = new Notification
                    {
                        Type = NotificationType.Success,
                        Message = "Record updated successfully!"
                    };
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    ViewData["Notification"] = new Notification
                    {
                        Type = NotificationType.Error,
                        Message = "Something went wrong!"
                    };
                }

                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var deleted = await articleRepository.DeleteAsync(ArticleRequest.Id);
            if (deleted)
            {
                await this._articleCacheProcessingChannel.ProcessArticleAsync(ArticleRequest.Id.ToString());
                var notification = new Notification
                {
                    Type = NotificationType.Success,
                    Message = "Blog was deleted successfully!"
                };

                TempData["Notification"] = JsonSerializer.Serialize(notification);

                return RedirectToPage("/Admin/Blogs/List");
            }

            return Page();
        }


        private void ValidateEditArticle()
        {
            if (!string.IsNullOrWhiteSpace(ArticleRequest.Heading) && ArticleRequest.Heading.Length is < 10 or > 72)
            {
                ModelState.AddModelError("Article.Heading",
                    "Heading can only be between 10 and 72 characters.");
            }
        }
    }
}
