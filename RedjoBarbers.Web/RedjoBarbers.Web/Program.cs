using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Configuration;
using RedjoBarbers.Web.Data.Models.Models;
using RedjoBarbers.Web.Services;
using RedjoBarbers.Web.Services.Contracts;

namespace RedjoBarbers.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        builder.Services
            .AddApplicationDbContext(connectionString)
            .AddApplicationIdentity()
            .AddApplicationServices()
            .AddApplicationMvc()
            .AddApplicationCookiePolicy();

        WebApplication app = builder.Build();

        await ApplyMigrationsAndSeedAsync(app);
        await AssignAdminRoleAsync(app, builder.Configuration);

        ConfigureMiddleware(app);
        ConfigureEndpoints(app);

        await app.RunAsync();
    }

    private static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        RedjoBarbersDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<RedjoBarbersDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    private static async Task AssignAdminRoleAsync(WebApplication app, IConfiguration configuration)
    {
        string? adminEmail = configuration["AdminSettings:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            return;
        }

        using IServiceScope scope = app.Services.CreateScope();

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        RoleManager<ApplicationRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        const string adminRoleName = "Admin";

        bool roleExists = await roleManager.RoleExistsAsync(adminRoleName);
        if (!roleExists)
        {
            IdentityResult roleCreationResult = await roleManager.CreateAsync(new ApplicationRole
            {
                Name = adminRoleName,
                NormalizedName = adminRoleName.ToUpperInvariant(),
                Label = adminRoleName
            });

            if (!roleCreationResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{adminRoleName}': {string.Join(", ", roleCreationResult.Errors.Select(e => e.Description))}");
            }
        }

        ApplicationUser? adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            return;
        }

        bool isAlreadyInRole = await userManager.IsInRoleAsync(adminUser, adminRoleName);
        if (isAlreadyInRole)
        {
            return;
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRoleName);
        if (!addToRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign role '{adminRoleName}' to user '{adminEmail}': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
        }
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error/500");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void ConfigureEndpoints(WebApplication app)
    {
        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<RedjoBarbersDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            options.UseSeeding((context, _) =>
            {
                RedjoBarbersDbContext dbContext = (RedjoBarbersDbContext)context;
                BarberServiceConfig.Seed(dbContext);
                BarberConfig.Seed(dbContext);
                RoleConfig.Seed(dbContext);
            });

            options.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                RedjoBarbersDbContext dbContext = (RedjoBarbersDbContext)context;
                await BarberServiceConfig.SeedAsync(dbContext, cancellationToken);
                await BarberConfig.SeedAsync(dbContext, cancellationToken);
                await RoleConfig.SeedAsync(dbContext, cancellationToken);
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<RedjoBarbersDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IBarberServiceService, BarberServiceService>();
        services.AddScoped<IHomeService, HomeService>();

        return services;
    }

    public static IServiceCollection AddApplicationMvc(this IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        services.AddRazorPages();

        return services;
    }

    public static IServiceCollection AddApplicationCookiePolicy(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => true;
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = HttpOnlyPolicy.None;
        });

        return services;
    }
}