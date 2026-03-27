using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;
using System.Security.Claims;

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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Services = await reviewService.GetAllServicesAsync();
                return View(model);
            }

            await reviewService.CreateAsync(model, userId);
            return RedirectToAction("MyAppointments", "Appointment");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                id,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                id,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                model.Id,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

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
