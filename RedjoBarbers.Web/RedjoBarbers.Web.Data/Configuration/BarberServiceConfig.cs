using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Data.Configuration
{
    /// <summary>
    /// Configures the BarberService entity and seeds initial service data.
    /// </summary>
    public sealed class BarberServiceConfig : IEntityTypeConfiguration<BarberService>
    {
        public void Configure(EntityTypeBuilder<BarberService> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(6,2)");

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }

        public static async Task SeedAsync(RedjoBarbersDbContext db, CancellationToken ct)
        {
            BarberService[] seed =
            [
                new BarberService
                {
                    Name = "Мъжко подстригване",
                    Description = "Подстригване на косата и стайлинг",
                    Price = 15.34m,
                    IsActive = true
                },
                new BarberService
                {
                    Name = "Мъжко подстригване + оформяне на брада",
                    Description = "Подстигване и стайлинг на косата, както и отделно оформяне на брадата",
                    Price = 20.45m,
                    IsActive = true
                },
                new BarberService
                {
                    Name = "Детско подстригване за момчета",
                    Description = "Подстригване и стайлинг на косата за момчета до 10 години",
                    Price = 12.78m,
                    IsActive = true
                },
                new BarberService
                {
                    Name = "Оформяне на брада",
                    Description = "Оформяне цялостната визия на брадата",
                    Price = 10.23m,
                    IsActive = true
                },
                new BarberService
                {
                    Name = "Оформяне на вежди",
                    Description = "Оформяне на вежди с бръснач за мъже",
                    Price = 2.56m,
                    IsActive = true
                },
            ];

            List<string> names = seed.Select((BarberService x) => x.Name).ToList();

            List<BarberService> existing = await db.BarberServices
                .Where((BarberService s) => names.Contains(s.Name))
                .ToListAsync(ct);

            foreach (BarberService s in seed)
            {
                BarberService? current = existing
                    .FirstOrDefault((BarberService e) => e.Name == s.Name);

                if (current == null)
                {
                    db.BarberServices.Add(s);
                }
                else
                {
                    current.Description = s.Description;
                    current.Price = s.Price;
                    current.IsActive = s.IsActive;
                }
            }

            await db.SaveChangesAsync(ct);
        }

        // Synchronous wrapper for async seeding
        public static void Seed(RedjoBarbersDbContext db)
        {
            SeedAsync(db, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
    }
}
