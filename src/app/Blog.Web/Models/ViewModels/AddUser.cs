using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models.ViewModels
{
    public class AddUser
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        public bool AdminCheckbox { get; set; }
    }
}
