using RedjoBarbers.Web.Data.Models.Enums;
using System;

namespace RedjoBarbers.Web.ViewModels.Appointments
{
    public class AppointmentListItemViewModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public string BarberName { get; set; } = null!;

        public string BarberServiceName { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }

        public string? Notes { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}