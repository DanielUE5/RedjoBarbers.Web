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
        public async Task<IActionResult> Index(int? barberServiceId, string? sortReviews, int currentPage = 1)
        {
            ReviewIndexPageViewModel model =
                await reviewService.GetAllAsync(barberServiceId, sortReviews, currentPage);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int? barberServiceId)
        {
            Guid? userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            ViewBag.Services = await reviewService.GetAllServicesAsync();

            ReviewCreateViewModel model =
                await reviewService.GetCreateModelAsync(barberServiceId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            Guid? userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Services = await reviewService.GetAllServicesAsync();
                return View(model);
            }

            await reviewService.CreateAsync(model, userId.Value);

            TempData["SuccessMessage"] = "Отзивът беше добавен успешно.";
            return RedirectToAction("MyAppointments", "Appointment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Guid? userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                id,
                userId.Value,
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

            TempData["SuccessMessage"] = "Отзивът беше изтрит успешно.";
            return RedirectToAction("MyAppointments", "Appointment");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            Guid? userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                id,
                userId.Value,
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
            Guid? userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            bool canAccess = await reviewService.IsOwnerOrAdminAsync(
                model.Id,
                userId.Value,
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

            TempData["SuccessMessage"] = "Отзивът беше обновен успешно.";
            return RedirectToAction("MyAppointments", "Appointment");
        }

        private Guid? GetCurrentUserId()
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out Guid userId))
            {
                return null;
            }

            return userId;
        }
    }
}