using Blog.Web.Enums;
using Blog.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        [BindProperty]
        public Register RegisterViewModel { get; set; } = null!;

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public void OnGet()
        {
            //// Empty get
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = RegisterViewModel.Username,
                    Email = RegisterViewModel.Email
                };

                var identityResult = await userManager.CreateAsync(user, RegisterViewModel.Password);

                if (identityResult.Succeeded)
                {
                    var addRolesResult = await userManager.AddToRoleAsync(user, "User");

                    if (addRolesResult.Succeeded)
                    {
                        ViewData["Notification"] = new Notification
                        {
                            Type = NotificationType.Success,
                            Message = "User registered successfully."
                        };

                        return Page();
                    }
                }

                ViewData["Notification"] = new Notification
                {
                    Type = NotificationType.Error,
                    Message = "Something went wrong."
                };

                return Page();
            }
            else
            {
                return Page();
            }
        }
    }
}
