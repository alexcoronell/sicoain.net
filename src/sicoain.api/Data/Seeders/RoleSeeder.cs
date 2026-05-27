using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace sicoain.api.Data.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            string[] roleNames = { "Admin", "Investigator", "Supervisor", "Consultant" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
                    await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName }).ConfigureAwait(false);
            }
        }
    }
}
