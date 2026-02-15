using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;

namespace RedjoBarbers.Web.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;

        public AppointmentController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Appointment> allAppointments = await dbContext.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Barber)
                .Include(appointment => appointment.BarberService)
                .AsSplitQuery()
                .OrderBy(appointment => appointment.AppointmentDate)
                .ToListAsync();

            return View(allAppointments);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Appointment? selectedAppointment = await dbContext.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Barber)
                .Include(appointment => appointment.BarberService)
                .AsSplitQuery()
                .FirstOrDefaultAsync(appointment => appointment.Id == id.Value);

            if (selectedAppointment is null)
            {
                return NotFound();
            }

            return View(selectedAppointment);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            AppointmentFormViewModel appointmentForm = new AppointmentFormViewModel
            {
                AppointmentDate = DateTime.Now
            };

            await PopulateDropdowns(appointmentForm);
            return View(appointmentForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(AppointmentFormViewModel appointmentForm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(appointmentForm);
                return View(appointmentForm);
            }

            bool timeSlotIsTaken = await HasTooManyAppointmentsInWindowAsync(
                appointmentForm.AppointmentDate,
                null,
                appointmentForm.BarberId);

            if (timeSlotIsTaken)
            {
                ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                    "Вече има запазен час в този диапазон. Диапазона за записване на час е 45мин. спрямо предишния.");

                await PopulateDropdowns(appointmentForm);
                return View(appointmentForm);
            }

            Appointment newAppointment = new Appointment
            {
                AppointmentDate = appointmentForm.AppointmentDate,
                CustomerName = appointmentForm.CustomerName,
                CustomerEmail = appointmentForm.CustomerEmail,
                CustomerPhone = appointmentForm.CustomerPhone,
                Notes = appointmentForm.Notes,
                Status = AppointmentStatus.Pending,
                BarberId = appointmentForm.BarberId,
                BarberServiceId = appointmentForm.BarberServiceId
            };

            dbContext.Appointments.Add(newAppointment);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(User.IsInRole("Admin") ? "Index" : nameof(MyAppointments)
);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Appointment? appointmentToEdit = await dbContext.Appointments.FindAsync(id.Value);

            if (appointmentToEdit is null)
            {
                return NotFound();
            }

            AppointmentFormViewModel appointmentForm = new AppointmentFormViewModel
            {
                Id = appointmentToEdit.Id,
                AppointmentDate = appointmentToEdit.AppointmentDate,
                CustomerName = appointmentToEdit.CustomerName,
                CustomerEmail = appointmentToEdit.CustomerEmail,
                CustomerPhone = appointmentToEdit.CustomerPhone,
                Notes = appointmentToEdit.Notes,
                Status = appointmentToEdit.Status,
                BarberId = appointmentToEdit.BarberId,
                BarberServiceId = appointmentToEdit.BarberServiceId
            };

            await PopulateDropdowns(appointmentForm);
            return View(appointmentForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(int id, AppointmentFormViewModel appointmentForm)
        {
            if (appointmentForm.Id is null || id != appointmentForm.Id.Value)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(appointmentForm);
                return View(appointmentForm);
            }

            bool timeSlotIsTaken = await HasTooManyAppointmentsInWindowAsync(
                appointmentForm.AppointmentDate,
                id,
                appointmentForm.BarberId);

            if (timeSlotIsTaken)
            {
                ModelState.AddModelError(nameof(appointmentForm.AppointmentDate),
                    "Вече има запазен час в този диапазон. Диапазона за записване на час е 45мин. спрямо предишния.");

                await PopulateDropdowns(appointmentForm);
                return View(appointmentForm);
            }

            Appointment? existingAppointment = await dbContext.Appointments.FindAsync(id);

            if (existingAppointment is null)
            {
                return NotFound();
            }

            existingAppointment.AppointmentDate = appointmentForm.AppointmentDate;
            existingAppointment.CustomerName = appointmentForm.CustomerName;
            existingAppointment.CustomerEmail = appointmentForm.CustomerEmail;
            existingAppointment.CustomerPhone = appointmentForm.CustomerPhone;
            existingAppointment.Notes = appointmentForm.Notes;
            existingAppointment.Status = appointmentForm.Status;
            existingAppointment.BarberId = appointmentForm.BarberId;
            existingAppointment.BarberServiceId = appointmentForm.BarberServiceId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Appointment? appointmentForDelete = await dbContext.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Barber)
                .Include(appointment => appointment.BarberService)
                .AsSplitQuery()
                .SingleOrDefaultAsync(appointment => appointment.Id == id.Value);

            if (appointmentForDelete is null)
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
            Appointment? appointmentToRemove = await dbContext.Appointments.FindAsync(id);

            if (appointmentToRemove is null)
            {
                return RedirectToAction(nameof(MyAppointments));
            }

            string customerPhone = appointmentToRemove.CustomerPhone;

            dbContext.Appointments.Remove(appointmentToRemove);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAppointments(string phone)
        {
            IEnumerable<Appointment> customerAppointments = await dbContext.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Barber)
                .Include(appointment => appointment.BarberService)
                .AsSplitQuery()
                .Where(appointment =>
                    string.IsNullOrWhiteSpace(phone) ||
                    appointment.CustomerPhone == phone)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ToListAsync();

            IEnumerable<Review> latestReviews = await dbContext.Reviews
                .AsNoTracking()
                .Include(review => review.BarberService)
                .AsSplitQuery()
                .OrderByDescending(review => review.ReviewDate)
                .ToListAsync();

            MyAppointmentsPageViewModel pageViewModel = new MyAppointmentsPageViewModel
            {
                Appointments = customerAppointments,
                Reviews = latestReviews
            };

            return View(pageViewModel);
        }

        private async Task PopulateDropdowns(AppointmentFormViewModel appointmentForm)
        {
            IEnumerable<SelectListItem> availableBarbers = await dbContext.Barbers
                .AsNoTracking()
                .OrderBy(barber => barber.Id)
                .Select(barber => new SelectListItem
                {
                    Value = barber.Id.ToString(),
                    Text = barber.Name
                })
                .ToListAsync();

            IEnumerable<SelectListItem> availableServices = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(service => service.Name)
                .Select(service => new SelectListItem
                {
                    Value = service.Id.ToString(),
                    Text = service.Name
                })
                .ToListAsync();

            appointmentForm.Barbers = availableBarbers;
            appointmentForm.BarberServices = availableServices;
        }

        private async Task<bool> HasTooManyAppointmentsInWindowAsync(
            DateTime targetDate,
            int? excludedAppointmentId,
            int? barberId)
        {
            DateTime windowStart = targetDate.AddMinutes(-45);
            DateTime windowEnd = targetDate.AddMinutes(45);

            IQueryable<Appointment> appointmentsInTimeRange = dbContext.Appointments
                .AsNoTracking()
                .Where(appointment =>
                    appointment.AppointmentDate >= windowStart &&
                    appointment.AppointmentDate <= windowEnd &&
                    appointment.Status != AppointmentStatus.Cancelled);

            if (excludedAppointmentId != null)
            {
                appointmentsInTimeRange =
                    appointmentsInTimeRange.Where(appointment =>
                        appointment.Id != excludedAppointmentId.Value);
            }

            if (barberId != null)
            {
                appointmentsInTimeRange =
                    appointmentsInTimeRange.Where(appointment =>
                        appointment.BarberId == barberId.Value);
            }

            return await appointmentsInTimeRange.AnyAsync();
        }
    }
}