using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models.ViewModels
{
    public class AddArticle
    {
        [Required]
        public string Heading { get; set; } = null!;

        [Required]
        public string PageTitle { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public string ShortDescription { get; set; } = null!;

        [Required]
        public string FeaturedImageUrl { get; set; } = null!;

        [Required]
        public string UrlHandle { get; set; } = null!;

        [Required]
        public DateTime PublishedDate { get; set; }
        [Required]
        public string Author { get; set; } = null!;

        public bool Visible { get; set; }
    }
}
