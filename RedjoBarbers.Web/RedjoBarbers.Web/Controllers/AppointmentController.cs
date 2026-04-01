using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;
using System.Security.Claims;

namespace RedjoBarbers.Web.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] AppointmentFilterViewModel filter)
        {
            AppointmentFilterViewModel model = await appointmentService.GetFilteredAsync(filter);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await appointmentService.IsOwnerOrAdminAsync(
                id.Value,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

            Appointment? appointment = await appointmentService.GetByIdAsync(id.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            AppointmentFormViewModel model = await appointmentService.GetCreateFormModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(AppointmentFormViewModel appointmentForm)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            AppointmentCreateResult result = await appointmentService.CreateAsync(appointmentForm, userId);

            switch (result)
            {
                case AppointmentCreateResult.InvalidBarberOrService:
                    ModelState.AddModelError(string.Empty, "Невалиден избор на бръснар или услуга.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);

                case AppointmentCreateResult.BusySlot:
                    ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                        "Вече има запазен час в този диапазон. Диапазонът за записване на час е 45 мин. спрямо предишния.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);

                case AppointmentCreateResult.Success:
                    return RedirectToAction(User.IsInRole("Admin") ? nameof(Index) : nameof(MyAppointments));

                default:
                    ModelState.AddModelError(string.Empty, "Възникна неочаквана грешка.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await appointmentService.IsOwnerOrAdminAsync(
                id.Value,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

            AppointmentFormViewModel? model = await appointmentService.GetFormModelByIdAsync(id.Value);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(int id, AppointmentFormViewModel appointmentForm)
        {
            if (appointmentForm.Id == null || id != appointmentForm.Id.Value)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await appointmentService.IsOwnerOrAdminAsync(
                id,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            AppointmentUpdateResult result = await appointmentService.UpdateAsync(id, appointmentForm);

            switch (result)
            {
                case AppointmentUpdateResult.NotFound:
                    return NotFound();

                case AppointmentUpdateResult.InvalidBarberOrService:
                    ModelState.AddModelError(string.Empty, "Невалиден избор на бръснар или услуга.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);

                case AppointmentUpdateResult.BusySlot:
                    ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                        "Вече има запазен час в този диапазон. Диапазонът за записване на час е 45 мин. спрямо предишния.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);

                case AppointmentUpdateResult.Success:
                    return RedirectToAction(nameof(MyAppointments));

                default:
                    ModelState.AddModelError(string.Empty, "Възникна неочаквана грешка.");
                    await appointmentService.PopulateDropdownsAsync(appointmentForm);
                    return View(appointmentForm);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool canAccess = await appointmentService.IsOwnerOrAdminAsync(
                id.Value,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

            Appointment? appointmentForDelete = await appointmentService.GetByIdAsync(id.Value);

            if (appointmentForDelete == null)
            {
                return NotFound();
            }

            return View(appointmentForDelete);
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

            bool canAccess = await appointmentService.IsOwnerOrAdminAsync(
                id,
                userId,
                User.IsInRole("Admin"));

            if (!canAccess)
            {
                return Forbid();
            }

            bool deleted = await appointmentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAppointments()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            MyAppointmentsPageViewModel vm = await appointmentService.GetMyAppointmentsPageAsync(userId);
            return View(vm);
        }
    }
}