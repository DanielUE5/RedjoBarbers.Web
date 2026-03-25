using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? barberServiceId)
        {
            IEnumerable<ReviewIndexItemViewModel> reviews = await reviewService.GetAllAsync(barberServiceId);
            return View(reviews);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int? barberServiceId)
        {
            ViewBag.Services = await reviewService.GetAllServicesAsync();
            ReviewCreateViewModel model = await reviewService.GetCreateModelAsync(barberServiceId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Services = await reviewService.GetAllServicesAsync();
                return View(model);
            }

            await reviewService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await reviewService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction("MyAppointments", "Appointment");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            ReviewUpdateViewModel? model = await reviewService.GetUpdateModelAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(ReviewUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool updated = await reviewService.UpdateAsync(model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("MyAppointments", "Appointment");
        }
    }
}
