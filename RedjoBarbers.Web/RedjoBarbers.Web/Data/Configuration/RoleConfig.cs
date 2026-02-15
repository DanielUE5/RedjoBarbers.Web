using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RedjoBarbers.Web.Data.Configuration
{
    public static class RoleConfig
    {
        public static async Task SeedAsync(RedjoBarbersDbContext context, CancellationToken ct)
        {
            DbSet<IdentityRole> roles = context.Set<IdentityRole>();

            string[] roleNames = ["Admin", "User"];

            foreach (string roleName in roleNames)
            {
                string normalized = roleName.ToUpperInvariant();

                bool exists = await roles.AnyAsync(r => r.NormalizedName == normalized, ct);
                if (!exists)
                {
                    IdentityRole role = new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = normalized
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