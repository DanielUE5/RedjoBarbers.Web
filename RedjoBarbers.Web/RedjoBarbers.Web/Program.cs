using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Configuration;

namespace RedjoBarbers.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<RedjoBarbersDbContext>(opt =>
            {
                /// <summary>
                /// I chose belt-and-suspenders seeding cause its recommended to implement both methods using similar logic.
                /// EF Core currently relies on the synchonous version of the method and will not seed the database correctly
                /// if the UseSeeding method is not implemented. (Information from the official documentation: https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
                /// </summary>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));

                opt.UseSeeding((context, migrated) =>
                {
                    BarberServiceConfig.Seed((RedjoBarbersDbContext)context);
                    BarberConfig.Seed((RedjoBarbersDbContext)context);
                    RoleConfig.Seed((RedjoBarbersDbContext)context);
                });

                opt.UseAsyncSeeding(async (context, migrated, ct) =>
                {
                    await BarberServiceConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                    await BarberConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                    await RoleConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                });
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;

                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<RedjoBarbersDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            WebApplication app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                RedjoBarbersDbContext db =
                    scope.ServiceProvider.GetRequiredService<RedjoBarbersDbContext>();

                await db.Database.MigrateAsync();

                string? adminEmail =
                    builder.Configuration["AdminSettings:Email"];

                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    UserManager<IdentityUser> userManager =
                        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    RoleManager<IdentityRole> roleManager =
                        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    const string adminRole = "Admin";

                    bool roleExists = await roleManager.RoleExistsAsync(adminRole);
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole(adminRole));
                    }

                    IdentityUser? user =
                        await userManager.FindByEmailAsync(adminEmail);

                    if (user != null)
                    {
                        bool isInRole =
                            await userManager.IsInRoleAsync(user, adminRole);

                        if (!isInRole)
                        {
                            await userManager.AddToRoleAsync(user, adminRole);
                        }
                    }
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();

            app.Run();
        }
    }
}
