using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data.Models.Models;

namespace RedjoBarbers.Web.Data.Configuration
{
    public static class RoleConfig
    {
        public static async Task SeedAsync(RedjoBarbersDbContext context, CancellationToken ct)
        {
            DbSet<ApplicationRole> roles = context.Set<ApplicationRole>();

            string[] roleNames = ["Admin", "User"];

            foreach (string roleName in roleNames)
            {
                string normalized = roleName.ToUpperInvariant();

                bool exists = await roles.AnyAsync(r => r.NormalizedName == normalized, ct);
                if (!exists)
                {
                    ApplicationRole role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = normalized,
                        Label = roleName
                    };

                    await roles.AddAsync(role, ct);
                }
            }

            await context.SaveChangesAsync(ct);
        }

        public static void Seed(RedjoBarbersDbContext db)
        {
            SeedAsync(db, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
    }
}