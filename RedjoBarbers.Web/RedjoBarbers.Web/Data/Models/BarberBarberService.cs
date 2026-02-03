namespace RedjoBarbers.Web.Data.Models
{
    public class BarberBarberService
    {
        // Composite Key: BarberId + BarberServiceId
        public int BarberId { get; set; }
        public Barber Barber { get; set; } = null!;

        public int BarberServiceId { get; set; }
        public BarberService BarberService { get; set; } = null!;
    }
}
