using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static RedjoBarbers.Web.Data.Common.EntityValidation.Appointment;

namespace RedjoBarbers.Web.ViewModels
{

    public class AppointmentFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Моля изберете дата и час.")]
        [Display(Name = "Дата")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Моля въведете име.")]
        [MaxLength(CustomerNameMaxLength)]
        [Display(Name = "Име на клиент")]
        public string CustomerName { get; set; } = null!;

        [MaxLength(CustomerEmailMaxLength)]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл адрес (по избор)")]
        public string? CustomerEmail { get; set; }

        [Required(ErrorMessage = "Моля въведете телефонен номер.")]
        [MaxLength(CustomerPhoneMaxLength)]
        [RegularExpression(PhoneRegexPattern, ErrorMessage = "Невалиден телефонен номер.")]
        [Phone]
        [Display(Name = "Телефонен номер")]
        public string CustomerPhone { get; set; } = null!;

        [MaxLength(NotesMaxLength)]
        [Display(Name = "Допълнителна информация (по избор)")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Моля изберете услуга.")]
        [Display(Name = "Изберете услуга")]
        public int BarberServiceId { get; set; }

        [Required(ErrorMessage = "Моля изберете бръснар.")]
        [Display(Name = "Изберете бръснар")]
        public int BarberId { get; set; }

        public IEnumerable<SelectListItem> BarberServices { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Barbers { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
