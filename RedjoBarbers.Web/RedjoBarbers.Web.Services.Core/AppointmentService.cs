using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.Services.Results;
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
            Appointment? appointment = await dbContext.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

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
                BarberId = appointment.BarberId,
                BarberServiceId = appointment.BarberServiceId
            };

            await PopulateDropdownsAsync(model);
            return model;
        }

        public async Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(string userId)
        {
            IEnumerable<Appointment> customerAppointments = await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            IEnumerable<Review> latestReviews = await dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.BarberService)
                .AsSplitQuery()
                .Where(r => r.UserId == userId)
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
                .OrderBy(b => b.Name)
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

        public async Task<AppointmentCreateResult> CreateAsync(AppointmentFormViewModel model, string userId)
        {
            bool isValidBarberAndService = await IsValidBarberAndServiceAsync(model.BarberId, model.BarberServiceId);

            if (!isValidBarberAndService)
            {
                return AppointmentCreateResult.InvalidBarberOrService;
            }

            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                null,
                model.BarberId);

            if (hasBusySlot)
            {
                return AppointmentCreateResult.BusySlot;
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
                BarberServiceId = model.BarberServiceId,
                UserId = userId
            };

            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();

            return AppointmentCreateResult.Success;
        }

        public async Task<bool> IsOwnerAsync(int appointmentId, string userId)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a => a.Id == appointmentId && a.UserId == userId);
        }

        public async Task<bool> IsOwnerOrAdminAsync(int appointmentId, string userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await dbContext.Appointments
                    .AsNoTracking()
                    .AnyAsync(a => a.Id == appointmentId);
            }

            return await dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a => a.Id == appointmentId && a.UserId == userId);
        }

        public async Task<AppointmentUpdateResult> UpdateAsync(int id, AppointmentFormViewModel model)
        {
            Appointment? appointment = await dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return AppointmentUpdateResult.NotFound;
            }

            bool isValidBarberAndService = await IsValidBarberAndServiceAsync(model.BarberId, model.BarberServiceId);

            if (!isValidBarberAndService)
            {
                return AppointmentUpdateResult.InvalidBarberOrService;
            }

            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                id,
                model.BarberId);

            if (hasBusySlot)
            {
                return AppointmentUpdateResult.BusySlot;
            }

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.CustomerName = model.CustomerName;
            appointment.CustomerEmail = model.CustomerEmail;
            appointment.CustomerPhone = model.CustomerPhone;
            appointment.Notes = model.Notes;
            appointment.BarberId = model.BarberId;
            appointment.BarberServiceId = model.BarberServiceId;

            await dbContext.SaveChangesAsync();

            return AppointmentUpdateResult.Success;
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

            IQueryable<Appointment> busyAppointmentsQuery = dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.AppointmentDate >= windowStart &&
                    a.AppointmentDate <= windowEnd &&
                    a.Status != AppointmentStatus.Cancelled);

            if (excludedAppointmentId.HasValue)
            {
                busyAppointmentsQuery = busyAppointmentsQuery.Where(a => a.Id != excludedAppointmentId.Value);
            }

            if (barberId.HasValue)
            {
                busyAppointmentsQuery = busyAppointmentsQuery.Where(a => a.BarberId == barberId.Value);
            }

            return await busyAppointmentsQuery.AnyAsync();
        }

        /// <summary>
        /// Filters, paginates, and populates appointment data based on the specified filter criteria.
        /// </summary>
        /// <remarks>The method updates the provided model with the filtered list of appointments, total
        /// count, and a list of available barbers for selection. If pagination parameters are not set or are out of
        /// range, default values are applied. The maximum page size is limited to 30 to prevent excessive data
        /// retrieval.</remarks>
        /// <param name="model">An AppointmentFilterViewModel containing the filter criteria, pagination settings, and properties to be
        /// populated with the filtered results. Must not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an AppointmentFilterViewModel
        /// with the filtered appointments, total count, and related barber selection data.</returns>
        public async Task<AppointmentFilterViewModel> GetFilteredAsync(AppointmentFilterViewModel model)
        {
            IQueryable<Appointment> filteredAppointmentsQuery = dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService);

            if (model.FromDate.HasValue)
            {
                DateTime fromDate = model.FromDate.Value.Date;
                filteredAppointmentsQuery = filteredAppointmentsQuery.Where(a => a.AppointmentDate >= fromDate);
            }

            if (model.ToDate.HasValue)
            {
                DateTime toDate = model.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                filteredAppointmentsQuery = filteredAppointmentsQuery.Where(a => a.AppointmentDate <= toDate);
            }

            if (model.Status.HasValue)
            {
                filteredAppointmentsQuery = filteredAppointmentsQuery.Where(a => a.Status == model.Status.Value);
            }

            if (model.BarberId.HasValue)
            {
                filteredAppointmentsQuery = filteredAppointmentsQuery.Where(a => a.BarberId == model.BarberId.Value);
            }

            model.TotalCount = await filteredAppointmentsQuery.CountAsync();

            if (model.Page < 1)
            {
                model.Page = 1;
            }

            if (model.PageSize <= 0)
            {
                model.PageSize = 8;
            }
            else if (model.PageSize > 30)
            {
                model.PageSize = 30;
            }

            model.Appointments = await filteredAppointmentsQuery
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.Id)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            model.Barbers = await dbContext.Barbers
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            return model;
        }

        private async Task<bool> IsValidBarberAndServiceAsync(int barberId, int barberServiceId)
        {
            bool barberExists = await dbContext.Barbers
                .AsNoTracking()
                .AnyAsync(b => b.Id == barberId);

            if (!barberExists)
            {
                return false;
            }

            bool serviceExists = await dbContext.BarberServices
                .AsNoTracking()
                .AnyAsync(s => s.Id == barberServiceId);

            if (!serviceExists)
            {
                return false;
            }

            return true;
        }
    }
}