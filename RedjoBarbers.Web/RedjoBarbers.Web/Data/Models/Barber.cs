using System.ComponentModel.DataAnnotations;
using static RedjoBarbers.Web.Common.EntityValidation.Barber;

namespace RedjoBarbers.Web.Data.Models
{
    public class Barber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(BarberNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(BarberBioMaxLength)]
        public string Bio { get; set; } = null!;

        [Required]
        [MaxLength(UrlMaxLength)]
        public string PhotoUrl { get; set; } = null!;

        [Required]
        [MaxLength(BarberPhoneNumberMaxLength)]
        [Url]
        public string PhoneNumber { get; set; } = null!;

        [Url]
        [MaxLength(UrlMaxLength)]
        public string? InstagramUrl { get; set; }

        [Url]
        [MaxLength(UrlMaxLength)]
        public string? FacebookUrl { get; set; }

        // Navigation properties
        public ICollection<BarberService> BarberServices { get; set; } = new List<BarberService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
