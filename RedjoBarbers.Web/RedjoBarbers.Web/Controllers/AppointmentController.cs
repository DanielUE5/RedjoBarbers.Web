using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;
using RedjoBarbers.Web.ViewModels.Appointments;
using System.Security.Claims;

public class AppointmentController : Controller
{
    private readonly IAppointmentService appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        this.appointmentService = appointmentService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IEnumerable<AppointmentListItemViewModel> model =
            await appointmentService.GetAllForListAsync();

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyAppointments()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        MyAppointmentsPageViewModel model =
            await appointmentService.GetMyAppointmentsPageAsync(userId);

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create(int? serviceId)
    {
        AppointmentFormViewModel model =
            await appointmentService.GetCreateFormModelAsync();

        if (serviceId.HasValue)
        {
            model.BarberServiceId = serviceId.Value;
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await appointmentService.PopulateDropdownsAsync(model);
            return View(model);
        }

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        AppointmentCreateResult result =
            await appointmentService.CreateAsync(model, userId);

        switch (result)
        {
            case AppointmentCreateResult.InvalidBarberOrService:
                ModelState.AddModelError(string.Empty, "Невалиден избор на бръснар или услуга.");
                break;

            case AppointmentCreateResult.BusySlot:
                ModelState.AddModelError(string.Empty, "Избраният час е зает.");
                break;

            case AppointmentCreateResult.Success:
                TempData["SuccessMessage"] = "Часът беше записан успешно.";
                return RedirectToAction(nameof(MyAppointments));
        }

        await appointmentService.PopulateDropdownsAsync(model);
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        bool isOwnerOrAdmin = await appointmentService.IsOwnerOrAdminAsync(id, userId, isAdmin);
        if (!isOwnerOrAdmin)
        {
            return Forbid();
        }

        AppointmentFormViewModel? model =
            await appointmentService.GetFormModelByIdAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, AppointmentFormViewModel model)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        bool isOwnerOrAdmin = await appointmentService.IsOwnerOrAdminAsync(id, userId, isAdmin);
        if (!isOwnerOrAdmin)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await appointmentService.PopulateDropdownsAsync(model);
            return View(model);
        }

        AppointmentUpdateResult result =
            await appointmentService.UpdateAsync(id, model);

        switch (result)
        {
            case AppointmentUpdateResult.NotFound:
                return NotFound();

            case AppointmentUpdateResult.InvalidBarberOrService:
                ModelState.AddModelError(string.Empty, "Невалиден избор на бръснар или услуга.");
                break;

            case AppointmentUpdateResult.BusySlot:
                ModelState.AddModelError(string.Empty, "Избраният час е зает.");
                break;

            case AppointmentUpdateResult.Success:
                TempData["SuccessMessage"] = "Часът беше обновен успешно.";
                return RedirectToAction(nameof(MyAppointments));
        }

        await appointmentService.PopulateDropdownsAsync(model);
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        bool isOwnerOrAdmin = await appointmentService.IsOwnerOrAdminAsync(id, userId, isAdmin);
        if (!isOwnerOrAdmin)
        {
            return Forbid();
        }

        AppointmentDetailsViewModel? model =
            await appointmentService.GetDetailsForDeleteAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(AppointmentDetailsViewModel model)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        bool isOwnerOrAdmin = await appointmentService.IsOwnerOrAdminAsync(model.Id, userId, isAdmin);
        if (!isOwnerOrAdmin)
        {
            return Forbid();
        }

        bool deleted = await appointmentService.DeleteAsync(model.Id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Часът беше изтрит успешно.";

        if (isAdmin)
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(MyAppointments));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots(DateTime date, int barberId, int barberServiceId)
    {
        IEnumerable<string> slots =
            await appointmentService.GetAvailableSlotsAsync(date, barberId, barberServiceId);

        return Json(slots);
    }
}