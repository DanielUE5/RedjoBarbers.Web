using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Data
{
    public class RedjoBarbersDbContext : DbContext
    {
        public RedjoBarbersDbContext(DbContextOptions<RedjoBarbersDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
            
        }

        public virtual DbSet<BarberService> BarberServices { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
    }
}
