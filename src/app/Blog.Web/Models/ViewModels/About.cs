namespace Blog.Web.Models.ViewModels
{
    public class About
    {
        public string Environment { get; set; } = null!;
        public string UseCache { get; set; } = null!;
        public string CacheSeconds { get; set; } = null!;
        public string ConnectionAuth { get; set; } = null!;
        public string ConnectionBlog { get; set; } = null!;
    }
}
