using RedjoBarbers.Web.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static RedjoBarbers.Web.Common.EntityValidation.Appointment;

namespace RedjoBarbers.Web.Data.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [MaxLength(CustomerNameMaxLength)]
        public string CustomerName { get; set; } = null!;

        [MaxLength(CustomerEmailMaxLength)]
        public string? CustomerEmail { get; set; }

        [Required]
        [MaxLength(CustomerPhoneMaxLength)]
        [RegularExpression(PhoneRegexPattern, ErrorMessage = "Invalid phone number.")]
        public string CustomerPhone { get; set; } = null!;

        [MaxLength(NotesMaxLength)]
        public string? Notes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // Foreign Key to BarberService
        public int BarberServiceId { get; set; }
        public BarberService BarberService { get; set; } = null!;

        // Foreign Key to Barber
        public int BarberId { get; set; }
        public Barber Barber { get; set; } = null!;
    }
}
