using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;
using RedjoBarbers.Web.ViewModels.Appointments;

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

        public async Task<IEnumerable<AppointmentListItemViewModel>> GetAllForListAsync()
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new AppointmentListItemViewModel
                {
                    Id = a.Id,
                    CustomerName = a.CustomerName,
                    BarberName = a.Barber.Name,
                    BarberServiceName = a.BarberService.Name,
                    AppointmentDate = a.AppointmentDate,
                    Notes = a.Notes,
                    Status = a.Status
                })
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

        public async Task<AppointmentDetailsViewModel?> GetDetailsForDeleteAsync(int id)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .AsSplitQuery()
                .Where(a => a.Id == id)
                .Select(a => new AppointmentDetailsViewModel
                {
                    Id = a.Id,
                    CustomerName = a.CustomerName,
                    CustomerEmail = a.CustomerEmail,
                    CustomerPhone = a.CustomerPhone,
                    BarberName = a.Barber.Name,
                    BarberServiceName = a.BarberService.Name,
                    AppointmentDate = a.AppointmentDate,
                    Notes = a.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AppointmentFormViewModel> GetCreateFormModelAsync()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel();

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

        public async Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(Guid userId)
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
                .OrderByDescending(s => s.Name == "Реджеп")
                .ThenBy(s => s.Name)
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

        public async Task<AppointmentCreateResult> CreateAsync(AppointmentFormViewModel model, Guid userId)
        {
            bool isValidBarberAndService = await IsValidBarberAndServiceAsync(model.BarberId, model.BarberServiceId);

            if (!isValidBarberAndService)
            {
                return AppointmentCreateResult.InvalidBarberOrService;
            }

            BarberService? barberService = await dbContext.BarberServices
                .AsNoTracking()
                .FirstOrDefaultAsync(bs => bs.Id == model.BarberServiceId);

            if (barberService == null)
            {
                return AppointmentCreateResult.InvalidBarberOrService;
            }

            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                barberService.DurationMinutes,
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
                UserId = userId,
                DurationMinutes = barberService.DurationMinutes
            };

            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();

            return AppointmentCreateResult.Success;
        }

        public async Task<bool> IsOwnerAsync(int appointmentId, Guid userId)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a => a.Id == appointmentId && a.UserId == userId);
        }

        public async Task<bool> IsOwnerOrAdminAsync(int appointmentId, Guid userId, bool isAdmin)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a =>
                    a.Id == appointmentId &&
                    (isAdmin || a.UserId == userId));
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

            BarberService? barberService = await dbContext.BarberServices
                .AsNoTracking()
                .FirstOrDefaultAsync(bs => bs.Id == model.BarberServiceId);

            if (barberService == null)
            {
                return AppointmentUpdateResult.InvalidBarberOrService;
            }

            bool hasBusySlot = await HasBusyTimeSlotAsync(
                model.AppointmentDate,
                barberService.DurationMinutes,
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
            appointment.DurationMinutes = barberService.DurationMinutes;

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

        public async Task<bool> HasBusyTimeSlotAsync(
            DateTime newStart,
            int newDurationMinutes,
            int? excludedAppointmentId,
            int? barberId)
        {
            DateTime dayStart = newStart.Date;
            DateTime dayEnd = dayStart.AddDays(1);
            DateTime newEnd = newStart.AddMinutes(newDurationMinutes);

            IQueryable<Appointment> appointmentsQuery = dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.AppointmentDate >= dayStart &&
                    a.AppointmentDate < dayEnd &&
                    a.Status != AppointmentStatus.Cancelled);

            if (excludedAppointmentId.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.Id != excludedAppointmentId.Value);
            }

            if (barberId.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.BarberId == barberId.Value);
            }

            List<Appointment> appointments = await appointmentsQuery.ToListAsync();

            return appointments.Any(a =>
            {
                DateTime existingStart = a.AppointmentDate;
                DateTime existingEnd = a.AppointmentDate.AddMinutes(a.DurationMinutes);

                return newStart < existingEnd && newEnd > existingStart;
            });
        }

        public async Task<IEnumerable<string>> GetAvailableSlotsAsync(DateTime date, int barberId, int barberServiceId)
        {
            if (date.DayOfWeek == DayOfWeek.Monday)
            {
                return Enumerable.Empty<string>();
            }

            BarberService? barberService = await dbContext.BarberServices
                .AsNoTracking()
                .FirstOrDefaultAsync(bs => bs.Id == barberServiceId);

            if (barberService == null)
            {
                return Enumerable.Empty<string>();
            }

            DateTime dayStart = date.Date;
            DateTime dayEnd = dayStart.AddDays(1);

            List<Appointment> appointments = await dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.BarberId == barberId &&
                    a.AppointmentDate >= dayStart &&
                    a.AppointmentDate < dayEnd &&
                    a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            List<string> availableSlots = new List<string>();

            TimeSpan workStart = new TimeSpan(10, 0, 0);
            TimeSpan workEnd = new TimeSpan(19, 0, 0);
            TimeSpan slotStep = TimeSpan.FromMinutes(15);
            TimeSpan serviceDuration = TimeSpan.FromMinutes(barberService.DurationMinutes);

            DateTime now = DateTime.Now;

            for (TimeSpan current = workStart; current < workEnd; current = current.Add(slotStep))
            {
                DateTime newStart = date.Date.Add(current);
                DateTime newEnd = newStart.Add(serviceDuration);

                if (newEnd.TimeOfDay > workEnd)
                {
                    continue;
                }

                if (date.Date == now.Date && newStart <= now)
                {
                    continue;
                }

                bool hasConflict = appointments.Any(a =>
                {
                    DateTime existingStart = a.AppointmentDate;
                    DateTime existingEnd = existingStart.AddMinutes(a.DurationMinutes);

                    return newStart < existingEnd && newEnd > existingStart;
                });

                if (!hasConflict)
                {
                    availableSlots.Add(newStart.ToString("HH:mm"));
                }
            }

            return availableSlots;
        }

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