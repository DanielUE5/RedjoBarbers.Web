using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

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
        public async Task<IActionResult> Index()
        {
            IEnumerable<Appointment> allAppointments = await appointmentService.GetAllAsync();
            return View(allAppointments);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
            if (!ModelState.IsValid)
            {
                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            bool created = await appointmentService.CreateAsync(appointmentForm);

            if (!created)
            {
                ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                    "Вече има запазен час в този диапазон. Диапазона за записване на час е 45мин. спрямо предишния.");

                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            return RedirectToAction(User.IsInRole("Admin") ? "Index" : nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
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

            if (!ModelState.IsValid)
            {
                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            bool updated = await appointmentService.UpdateAsync(id, appointmentForm);

            if (!updated)
            {
                ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                    "Вече има запазен час в този диапазон. Диапазона за записване на час е 45мин. спрямо предишния.");

                await appointmentService.PopulateDropdownsAsync(appointmentForm);
                return View(appointmentForm);
            }

            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
            await appointmentService.DeleteAsync(id);
            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAppointments(string phone)
        {
            MyAppointmentsPageViewModel vm = await appointmentService.GetMyAppointmentsPageAsync(phone);
            return View(vm);
        }
    }
}