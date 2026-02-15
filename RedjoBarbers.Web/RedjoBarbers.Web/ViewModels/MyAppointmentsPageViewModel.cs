using RedjoBarbers.Web.Data.Models;

public class MyAppointmentsPageViewModel
{
    // This view model represents the data needed for the "My Appointments" page, including the user's appointments and reviews.
    public IEnumerable<Appointment> Appointments { get; set; } = Enumerable.Empty<Appointment>();
    public IEnumerable<Review> Reviews { get; set; } = Enumerable.Empty<Review>();
}
