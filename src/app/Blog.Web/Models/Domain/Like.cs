namespace Blog.Web.Models.Domain
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
}
