using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Moderator, Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public List<User> Users { get; set; } = null!;

        [BindProperty]
        public AddUser AddUserRequest { get; set; } = null!;

        [BindProperty]
        public Guid SelectedUserId { get; set; }

        public IndexModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> OnGet()
        {
            await GetUsers();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = AddUserRequest.Username,
                    Email = AddUserRequest.Email
                };

                var roles = new List<string> { "User" };

                if (AddUserRequest.AdminCheckbox)
                {
                    roles.Add("Moderator");
                }

                var result = await userRepository.Add(identityUser, AddUserRequest.Password, roles);

                if (result)
                {
                    return RedirectToPage("/Admin/Users/Index");
                }

                return Page();
            }

            await GetUsers();
            return Page();
        }


        public async Task<IActionResult> OnPostDelete()
        {
            await userRepository.Delete(SelectedUserId);
            return RedirectToPage("/Admin/Users/Index");
        }


        private async Task GetUsers()
        {
            var users = await userRepository.GetAll();

            Users = new List<User>();
            foreach (var user in users)
            {
                Users.Add(new User()
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName!,
                    Email = user.Email!
                });
            }
        }

    }
}
