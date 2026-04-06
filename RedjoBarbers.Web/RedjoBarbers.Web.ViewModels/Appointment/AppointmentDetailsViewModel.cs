using System;

namespace RedjoBarbers.Web.ViewModels.Appointments
{
    public class AppointmentDetailsViewModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public string BarberName { get; set; } = null!;

        public string BarberServiceName { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }

        public string? Notes { get; set; }
    }
}