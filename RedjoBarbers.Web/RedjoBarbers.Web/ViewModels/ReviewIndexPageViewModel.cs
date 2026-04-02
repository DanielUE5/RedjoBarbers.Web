using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedjoBarbers.Web.ViewModels
{
    public class ReviewIndexPageViewModel
    {
        public int? BarberServiceId { get; set; }

        public string? SortReviews { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 6;

        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();

        public IEnumerable<ReviewIndexItemViewModel> Reviews { get; set; } = new List<ReviewIndexItemViewModel>();
    }
}