using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.ViewModels
{
    public class AdminPanelViewModel
    {
        public IReadOnlyList<Appointment> Appointments { get; init; } = new List<Appointment>();
        public IReadOnlyList<Review> Reviews { get; init; } = new List<Review>();
    }
}