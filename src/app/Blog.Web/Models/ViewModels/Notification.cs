using Blog.Web.Enums;

namespace Blog.Web.Models.ViewModels
{
    public class Notification
    {
        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }
    }
}
