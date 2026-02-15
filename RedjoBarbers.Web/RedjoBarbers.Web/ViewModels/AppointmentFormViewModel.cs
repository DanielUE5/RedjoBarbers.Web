using Microsoft.AspNetCore.Mvc.Rendering;
using RedjoBarbers.Web.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using static RedjoBarbers.Web.Common.EntityValidation.Appointment;

public class AppointmentFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Моля изберете дата и час.")]
    [Display(Name = "Дата и час")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
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

    [Display(Name = "Статус")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [Required(ErrorMessage = "Моля изберете услуга.")]
    [Display(Name = "Изберете услуга")]
    public int BarberServiceId { get; set; }

    [Required(ErrorMessage = "Моля изберете бръснар.")]
    [Display(Name = "Изберете бръснар")]
    public int BarberId { get; set; }

    public IEnumerable<SelectListItem> BarberServices { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Barbers { get; set; } = Enumerable.Empty<SelectListItem>();
}
