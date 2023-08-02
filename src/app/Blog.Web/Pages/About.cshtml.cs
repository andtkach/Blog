using Blog.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages
{
    [Authorize(Roles = "Moderator, Administrator")]
    public class AboutModel : PageModel
    {
        private readonly IConfiguration configuration;

        [BindProperty]
        public About About { get; set; } = null!;

        public AboutModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            var result = new About()
            {
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!,
                ConnectionBlog = configuration.GetConnectionString("BlogDbConnectionString")!,
                ConnectionAuth = configuration.GetConnectionString("AuthDbConnectionString")!,
                Cache = Environment.GetEnvironmentVariable("ContentCache:UseCache")!
            };
            About = result;
        }
    }
}
