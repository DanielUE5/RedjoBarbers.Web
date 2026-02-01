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
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));

                // Belt-and-suspenders seeding approach
                opt.UseSeeding((context, migrated) =>
                {
                    BarberServiceConfig.Seed((RedjoBarbersDbContext)context);
                });

                opt.UseAsyncSeeding(async (context, migrated, ct) =>
                {
                    await BarberServiceConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                });
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            WebApplication app = builder.Build();

            /// <summary>
            /// Here we apply any pending migrations automatically during development.
            /// </summary>
            if (app.Environment.IsDevelopment())
            {
                using IServiceScope scope = app.Services.CreateScope();
                RedjoBarbersDbContext db = scope.ServiceProvider.GetRequiredService<RedjoBarbersDbContext>();

                IEnumerable<string> pendingMigrations = await db.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    await db.Database.MigrateAsync();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
