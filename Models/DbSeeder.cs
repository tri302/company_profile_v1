using Microsoft.AspNetCore.Identity;

namespace company_profile.Models
{
    public class DbSeeder
    {
        public static async Task SeedSuperAdminAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "SuperAdmin",
                "Admin",
                "User"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            var email = "superadmin@gmail.com";

            var user =
                await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var superAdmin = new ApplicationUser
                {
                    FullName = "Super Admin",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(
                        superAdmin,
                        "SuperAdmin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        superAdmin,
                        "SuperAdmin");
                }
            }
        }
    }
}