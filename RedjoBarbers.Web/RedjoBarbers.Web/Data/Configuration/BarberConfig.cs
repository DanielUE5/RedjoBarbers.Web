using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Data.Configuration
{
    public class BarberConfig : IEntityTypeConfiguration<Barber>
    {
        public void Configure(EntityTypeBuilder<Barber> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Bio)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(b => b.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(b => b.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.InstagramUrl)
                .HasMaxLength(2048);

            builder.Property(b => b.FacebookUrl)
                .HasMaxLength(2048);
        }

        public static async Task SeedAsync(RedjoBarbersDbContext db, CancellationToken ct)
        {
            Barber[] seed =
            [
                new Barber
        {
            Name = "Реджеп",
            Bio = "Специалист в класическите подстрижки и оформяне на брада.",
            PhotoUrl = "/images/barbers/example1.jpg",
            PhoneNumber = "+359888123456",
            InstagramUrl = "https://www.instagram.com/_buona_note_/",
            FacebookUrl = null
        },
        new Barber
        {
            Name = "Илиян",
            Bio = "Фейдове, модерни стилове и детайлна работа по контур.",
            PhotoUrl = "/images/barbers/example2.jpg",
            PhoneNumber = "+359888654321",
            InstagramUrl = null,
            FacebookUrl = null
        },
            ];

            List<string> phones = seed.Select(b => b.PhoneNumber).ToList();

            List<Barber> existing = await db.Barbers
                .Where(b => phones.Contains(b.PhoneNumber))
                .ToListAsync(ct);

            foreach (Barber b in seed)
            {
                Barber? current = existing
                    .FirstOrDefault(e => e.PhoneNumber == b.PhoneNumber);

                if (current == null)
                {
                    db.Barbers.Add(b);
                }
                else
                {
                    current.Name = b.Name;
                    current.Bio = b.Bio;
                    current.PhotoUrl = b.PhotoUrl;
                    current.InstagramUrl = b.InstagramUrl;
                    current.FacebookUrl = b.FacebookUrl;
                }
            }

            await db.SaveChangesAsync(ct);
        }

        public static void Seed(RedjoBarbersDbContext db)
        {
            SeedAsync(db, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
    }
}
