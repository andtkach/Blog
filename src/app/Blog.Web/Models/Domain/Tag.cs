namespace Blog.Web.Models.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public Guid ArticleId { get; set; }
    }
}
