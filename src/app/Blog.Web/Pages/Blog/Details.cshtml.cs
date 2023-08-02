using System.ComponentModel.DataAnnotations;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly IArticleRepository articleRepository;
        private readonly ILikeRepository likeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ICommentRepository commentRepository;

        public Article Article { get; set; } = null!;

        public List<BlogComment> Comments { get; set; } = null!;

        public int TotalLikes { get; set; }
        public bool Liked { get; set; }

        [BindProperty]
        public Guid ArticleId { get; set; }

        [BindProperty]
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string CommentDescription { get; set; } = null!;


        public DetailsModel(IArticleRepository articleRepository,
            ILikeRepository likeRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ICommentRepository commentRepository)
        {
            this.articleRepository = articleRepository;
            this.likeRepository = likeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.commentRepository = commentRepository;
        }

        public async Task<IActionResult> OnGet(string urlHandle)
        {
            await GetBlog(urlHandle);
            return Page();
        }

        public async Task<IActionResult> OnPost(string urlHandle)
        {
            if (ModelState.IsValid)
            {
                if (signInManager.IsSignedIn(User) && !string.IsNullOrWhiteSpace(CommentDescription))
                {
                    var userId = userManager.GetUserId(User);

                    var comment = new Comment()
                    {
                        ArticleId = ArticleId,
                        Description = CommentDescription,
                        DateAdded = DateTime.Now,
                        
                    };

                    if (userId != null)
                    {
                        comment.UserId = Guid.Parse(userId);
                    }

                    await commentRepository.AddAsync(comment);
                }

                return RedirectToPage("/Blog/Details", new { urlHandle = urlHandle });
            }

            await GetBlog(urlHandle);
            return Page();
        }

        private async Task GetComments()
        {
            var comments = await commentRepository.GetAllAsync(Article.Id);

            var blogCommentsViewModel = new List<BlogComment>();
            foreach (var comment in comments)
            {

                var newItem = new BlogComment
                {
                    DateAdded = comment.DateAdded,
                    Description = comment.Description
                };

                var user = await userManager.FindByIdAsync(comment.UserId.ToString());
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    newItem.Username = user.UserName;
                }

                blogCommentsViewModel.Add(newItem);
            }

            Comments = blogCommentsViewModel;
        }

        private async Task GetBlog(string urlHandle)
        {
            var article = await articleRepository.GetAsync(urlHandle);
            
            if (article != null)
            {
                Article = article; 
                ArticleId = Article.Id;
                if (signInManager.IsSignedIn(User))
                {
                    var likes = await likeRepository.GetLikesForBlog(Article.Id);

                    var userId = userManager.GetUserId(User);
                    if (userId != null)
                    {
                        Liked = likes.Any(x => x.UserId == Guid.Parse(userId));
                    }

                    await GetComments();
                }

                TotalLikes = await likeRepository.GetTotalLikesForBlog(Article.Id);
            }
        }
    }
}
