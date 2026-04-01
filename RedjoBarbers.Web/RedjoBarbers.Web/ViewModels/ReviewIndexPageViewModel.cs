using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedjoBarbers.Web.ViewModels
{
    public class ReviewIndexViewModel
    {
        public int? BarberServiceId { get; set; }

        public string? SortReviews { get; set; }

        public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();

        public IEnumerable<ReviewIndexItemViewModel> Reviews { get; set; } = new List<ReviewIndexItemViewModel>();
    }
}