using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;
using RedjoBarbers.Web.ViewModels.Appointments;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IAppointmentService
    {
        Task<Appointment?> GetByIdAsync(int id);

        Task<AppointmentFormViewModel> GetCreateFormModelAsync();

        Task<AppointmentFormViewModel?> GetFormModelByIdAsync(int id);

        Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(Guid userId);

        Task PopulateDropdownsAsync(AppointmentFormViewModel model);

        Task<AppointmentCreateResult> CreateAsync(AppointmentFormViewModel model, Guid userId);

        Task<bool> IsOwnerAsync(int appointmentId, Guid userId);

        Task<bool> IsOwnerOrAdminAsync(int appointmentId, Guid userId, bool isAdmin);

        Task<AppointmentUpdateResult> UpdateAsync(int id, AppointmentFormViewModel model);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasBusyTimeSlotAsync(DateTime newStart, int newDurationMinutes, int? excludedAppointmentId, int? barberId);

        Task<IEnumerable<string>> GetAvailableSlotsAsync(DateTime date, int barberId, int barberServiceId);

        Task<AppointmentFilterViewModel> GetFilteredAsync(AppointmentFilterViewModel model);

        Task<IEnumerable<AppointmentListItemViewModel>> GetAllForListAsync();

        Task<AppointmentDetailsViewModel?> GetDetailsForDeleteAsync(int id);
    }
}