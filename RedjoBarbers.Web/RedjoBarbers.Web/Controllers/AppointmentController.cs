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
        public async Task<IActionResult> Index()
        {
            IEnumerable<Appointment> appointments = await dbContext
            .Appointments
            .AsNoTracking()
            .Include(a => a.Barber)
            .Include(a => a.BarberService)
            .AsSplitQuery()
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            Appointment? detail = await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .FirstOrDefaultAsync(a => a.Id == id.Value);

            if (detail is null) return NotFound();

            return View(detail);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            AppointmentFormViewModel vm = new AppointmentFormViewModel
            {
                AppointmentDate = DateTime.Now
            };

            await PopulateDropdowns(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            Appointment? entity = new Appointment
            {
                AppointmentDate = vm.AppointmentDate,
                CustomerName = vm.CustomerName,
                CustomerEmail = vm.CustomerEmail,
                CustomerPhone = vm.CustomerPhone,
                Notes = vm.Notes,
                Status = AppointmentStatus.Pending,
                BarberId = vm.BarberId,
                BarberServiceId = vm.BarberServiceId
            };

            dbContext.Appointments.Add(entity);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) 
            {
                return NotFound();
            }

            Appointment? appointment = await dbContext
                .Appointments
                .FindAsync(id.Value);

            if (appointment is null) return NotFound();

            AppointmentFormViewModel vm = new AppointmentFormViewModel
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                CustomerName = appointment.CustomerName,
                CustomerEmail = appointment.CustomerEmail,
                CustomerPhone = appointment.CustomerPhone,
                Notes = appointment.Notes,
                Status = AppointmentStatus.Pending,
                BarberId = appointment.BarberId,
                BarberServiceId = appointment.BarberServiceId
            };

            await PopulateDropdowns(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, AppointmentFormViewModel vm)
        {
            if (vm.Id is null || id != vm.Id.Value) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            Appointment? entity = await dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (entity is null) return NotFound();

            entity.AppointmentDate = vm.AppointmentDate;
            entity.CustomerName = vm.CustomerName;
            entity.CustomerEmail = vm.CustomerEmail;
            entity.CustomerPhone = vm.CustomerPhone;
            entity.Notes = vm.Notes;
            entity.Status = vm.Status;
            entity.BarberId = vm.BarberId;
            entity.BarberServiceId = vm.BarberServiceId;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            Appointment? item = await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .SingleOrDefaultAsync(a => a.Id == id.Value);

            if (item is null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Appointment? entity = await dbContext.Appointments.FindAsync(id);
            if (entity == null)
                return RedirectToAction(nameof(Index));

            dbContext.Appointments.Remove(entity);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates the barbers and barber services dropdown lists in the specified appointment form view model.
        /// </summary>
        /// <remarks>This method retrieves the lists of barbers and barber services asynchronously from
        /// the database and assigns them to the corresponding properties of the view model. The data is retrieved
        /// without tracking to improve performance when the entities are not being updated.</remarks>
        /// <param name="vm">The view model that contains the dropdown lists to be populated with available barbers and barber services.</param>
        /// <returns></returns>
        private async Task PopulateDropdowns(AppointmentFormViewModel vm)
        {
            vm.Barbers = await dbContext.Barbers
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            vm.BarberServices = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }
    }
}