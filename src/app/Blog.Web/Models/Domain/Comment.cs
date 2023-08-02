namespace Blog.Web.Models.Domain
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = null!;

        public Guid ArticleId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
