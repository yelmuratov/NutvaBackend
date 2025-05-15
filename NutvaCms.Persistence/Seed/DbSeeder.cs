using Microsoft.EntityFrameworkCore;
using NutvaCms.Persistence.DbContexts;

namespace NutvaCms.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedSuperAdminAsync(AppDbContext context)
    {
        if (!await context.Admins.AnyAsync())
        {
            var superAdmin = new Admin
            {
                Email = "admin@nutva.uz",
                Password = BCrypt.Net.BCrypt.HashPassword("SuperSecure123!"),
                IsSuperAdmin = true
            };

            context.Admins.Add(superAdmin);
            await context.SaveChangesAsync();
        }
    }
}
