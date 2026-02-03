using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static RedjoBarbers.Web.Common.EntityValidation.BarberService;

namespace RedjoBarbers.Web.Data.Models
{
    public class BarberService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(BarberServiceNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(BarberServiceDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Column(TypeName = PriceColumnType)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Barber> Barbers { get; set; } = new List<Barber>();
    }
}
