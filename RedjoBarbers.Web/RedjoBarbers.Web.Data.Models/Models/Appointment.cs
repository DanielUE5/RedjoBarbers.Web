using Microsoft.AspNetCore.Identity;
using RedjoBarbers.Web.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static RedjoBarbers.Web.Data.Common.EntityValidation.Appointment;

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
        [RegularExpression(PhoneRegexPattern, ErrorMessage = "Невалиден телефонен номер.")]
        public string CustomerPhone { get; set; } = null!;

        [MaxLength(NotesMaxLength)]
        public string? Notes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public int DurationMinutes { get; set; }

        // Foreign Key to BarberService
        public int BarberServiceId { get; set; }
        public BarberService BarberService { get; set; } = null!;

        // Foreign Key to Barber
        public int BarberId { get; set; }
        public Barber Barber { get; set; } = null!;

        // Foreign Key to IdentityUser (Customer)
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
