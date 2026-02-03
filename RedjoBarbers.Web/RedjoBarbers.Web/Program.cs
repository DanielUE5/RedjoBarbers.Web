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

                /// <summary>
                /// I chose belt-and-suspenders seeding cause its recommended to implement both methods using similar logic.
                /// EF Core currently relies on the synchonous version of the method and will not seed the database correctly
                /// if the UseSeeding method is not implemented. (Information from the official documentation: https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
                /// </summary>

                opt.UseSeeding((context, migrated) =>
                {
                    BarberServiceConfig.Seed((RedjoBarbersDbContext)context);
                    BarberConfig.Seed((RedjoBarbersDbContext)context);
                });

                opt.UseAsyncSeeding(async (context, migrated, ct) =>
                {
                    await BarberServiceConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                    await BarberConfig.SeedAsync((RedjoBarbersDbContext)context, ct);
                });
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using IServiceScope scope = app.Services.CreateScope();
                RedjoBarbersDbContext db = scope.ServiceProvider.GetRequiredService<RedjoBarbersDbContext>();

                await db.Database.MigrateAsync();
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
