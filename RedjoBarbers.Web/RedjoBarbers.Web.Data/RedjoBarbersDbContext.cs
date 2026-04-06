using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Models;

namespace RedjoBarbers.Web.Data
{
    public class RedjoBarbersDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public RedjoBarbersDbContext(DbContextOptions<RedjoBarbersDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
            
        }

        public virtual DbSet<BarberService> BarberServices { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Barber> Barbers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RedjoBarbersDbContext).Assembly);
        }
    }
}
