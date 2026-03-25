using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly RedjoBarbersDbContext dbContext;
        public AppointmentService(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AppointmentFormViewModel> GetCreateFormModelAsync()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = DateTime.Now
            };

            await PopulateDropdownsAsync(model);
            return model;
        }

        public async Task<AppointmentFormViewModel?> GetFormModelByIdAsync(int id)
        {
            Appointment? appointment = await dbContext.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return null;
            }

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                CustomerName = appointment.CustomerName,
                CustomerEmail = appointment.CustomerEmail,
                CustomerPhone = appointment.CustomerPhone,
                Notes = appointment.Notes,
                Status = appointment.Status,
                BarberId = appointment.BarberId,
                BarberServiceId = appointment.BarberServiceId
            };

            await PopulateDropdownsAsync(model);

            return model;
        }

        public async Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(string? phone)
        {
            IEnumerable<Appointment> customerAppointments = await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .Where(a => string.IsNullOrWhiteSpace(phone) || a.CustomerPhone == phone)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            IEnumerable<Review> latestReviews = await dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.BarberService)
                .AsSplitQuery()
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return new MyAppointmentsPageViewModel
            {
                Appointments = customerAppointments,
                Reviews = latestReviews
            };
        }

        public async Task PopulateDropdownsAsync(AppointmentFormViewModel model)
        {
            model.Barbers = await dbContext.Barbers
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            model.BarberServices = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(AppointmentFormViewModel model)
        {
            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                null,
                model.BarberId);

            if (hasBusySlot)
            {
                return false;
            }

            Appointment appointment = new Appointment
            {
                AppointmentDate = model.AppointmentDate,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                Notes = model.Notes,
                Status = AppointmentStatus.Pending,
                BarberId = model.BarberId,
                BarberServiceId = model.BarberServiceId
            };

            dbContext.Appointments.Add(appointment);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(int id, AppointmentFormViewModel model)
        {
            Appointment? appointment = await dbContext.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return false;
            }

            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                id,
                model.BarberId);

            if (hasBusySlot)
            {
                return false;
            }

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.CustomerName = model.CustomerName;
            appointment.CustomerEmail = model.CustomerEmail;
            appointment.CustomerPhone = model.CustomerPhone;
            appointment.Notes = model.Notes;
            appointment.Status = model.Status;
            appointment.BarberId = model.BarberId;
            appointment.BarberServiceId = model.BarberServiceId;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Appointment? appointment = await dbContext.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return false;
            }

            dbContext.Appointments.Remove(appointment);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbContext.Appointments.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> HasBusyTimeSlotAsync(DateTime targetDate, int? excludedAppointmentId, int? barberId)
        {
            DateTime windowStart = targetDate.AddMinutes(-45);
            DateTime windowEnd = targetDate.AddMinutes(45);

            IQueryable<Appointment> query = dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.AppointmentDate >= windowStart &&
                    a.AppointmentDate <= windowEnd &&
                    a.Status != AppointmentStatus.Cancelled);

            if (excludedAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludedAppointmentId.Value);
            }

            if (barberId.HasValue)
            {
                query = query.Where(a => a.BarberId == barberId.Value);
            }

            return await query.AnyAsync();
        }
    }
}