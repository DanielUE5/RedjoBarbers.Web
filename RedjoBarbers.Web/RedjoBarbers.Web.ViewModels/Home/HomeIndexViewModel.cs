using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<BarberService> Services { get; init; } = new List<BarberService>();
        public IReadOnlyList<Review> Reviews { get; init; } = new List<Review>();
    }
}