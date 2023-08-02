using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models.ViewModels
{
    public class Login
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
