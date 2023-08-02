using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var administratorRoleId = "589465c3-7396-46a4-8bf0-40203342faae";
            var moderatorRoleId = "aa2cf69c-a91a-4967-ba8c-18b8f6d54f60";
            var userRoleId = "a446eafe-5096-4d8c-889c-b4c92f5d6e63";

            // Seed Roles (User, Moderator, Administrator)
            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "Administrator",
                    NormalizedName = "Administrator",
                    Id = administratorRoleId,
                    ConcurrencyStamp = administratorRoleId
                },
                new IdentityRole()
                {
                    Name = "Moderator",
                    NormalizedName = "Moderator",
                    Id = moderatorRoleId,
                    ConcurrencyStamp = moderatorRoleId
                },
                new IdentityRole()
                {
                    Name = "User",
                    NormalizedName = "User",
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            // Seed Super Admin User
            var administratorId = "c052ffb4-245b-4c3c-8390-f1bd806828e2";
            var administratorUser = new IdentityUser()
            {
                Id = administratorId,
                UserName = "admin@blog.com",
                Email = "admin@blog.com",
                NormalizedEmail = "admin@blog.com".ToUpper(),
                NormalizedUserName = "admin@blog.com".ToUpper()
            };

            administratorUser.PasswordHash = new PasswordHasher<IdentityUser>()
                                    .HashPassword(administratorUser, "admin123");

            builder.Entity<IdentityUser>().HasData(administratorUser);

            // Add All Roles To Administrator User
            var administratorRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>
                {
                    RoleId = administratorRoleId,
                    UserId = administratorId
                },
                new IdentityUserRole<string>
                {
                    RoleId = moderatorRoleId,
                    UserId = administratorId
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = administratorId
                },
            };

            builder.Entity<IdentityUserRole<string>>().HasData(administratorRoles);
        }
    }
}
