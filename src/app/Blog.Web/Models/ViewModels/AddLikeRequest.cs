namespace Blog.Web.Models.ViewModels
{
    public class AddLikeRequest
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
}
