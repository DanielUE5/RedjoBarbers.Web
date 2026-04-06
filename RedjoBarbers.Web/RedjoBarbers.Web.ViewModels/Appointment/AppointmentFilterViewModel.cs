using Microsoft.AspNetCore.Mvc.Rendering;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.ViewModels
{
    public class AppointmentFilterViewModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public AppointmentStatus? Status { get; set; } 

        public int? BarberId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 8;

        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

        public IEnumerable<SelectListItem> Barbers { get; set; } = new List<SelectListItem>();
    }
}