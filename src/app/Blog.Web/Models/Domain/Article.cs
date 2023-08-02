namespace Blog.Web.Models.Domain
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Heading { get; set; } = null!;
        public string PageTitle { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string FeaturedImageUrl { get; set; } = null!;
        public string UrlHandle { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; } = null!;
        public bool Visible { get; set; }

        // Navigation Property
        public ICollection<Tag> Tags { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}
