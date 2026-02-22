using Microsoft.AspNetCore.Identity;
using TenderingSystem.Infrastructure.Identity;

namespace TenderingSystem.Infrastructure.Data.Seed;

public static class IdentitySeeder
{
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Create Roles if they don't exist
        var roles = new[] { "Admin", "ProcurementManager", "Supplier" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Create the Default Admin User
        var adminEmail = "mostafa.business97@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
            {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Mostafa",
                LastName = "Ashraf",
                EmailConfirmed = true
            };

            // 3. Assign password
            var result = await userManager.CreateAsync(adminUser, "246810");
            
            // 4. Assign Admin Role
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
