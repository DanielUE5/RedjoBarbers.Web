using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<BarberService> Services { get; set; } = new List<BarberService>();
        public IReadOnlyList<Review> Reviews { get; set; } = new List<Review>();
    }
}