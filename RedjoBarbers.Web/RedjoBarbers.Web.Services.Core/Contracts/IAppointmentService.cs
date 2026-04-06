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

        Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(string userId);

        Task PopulateDropdownsAsync(AppointmentFormViewModel model);

        Task<AppointmentCreateResult> CreateAsync(AppointmentFormViewModel model, string userId);

        Task<bool> IsOwnerAsync(int appointmentId, string userId);

        Task<bool> IsOwnerOrAdminAsync(int appointmentId, string userId, bool isAdmin);

        Task<AppointmentUpdateResult> UpdateAsync(int id, AppointmentFormViewModel model);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasBusyTimeSlotAsync(DateTime newStart, int newDurationMinutes, int? excludedAppointmentId, int? barberId);

        Task<IEnumerable<string>> GetAvailableSlotsAsync(DateTime date, int barberId, int barberServiceId);

        Task<AppointmentFilterViewModel> GetFilteredAsync(AppointmentFilterViewModel model);

        Task<IEnumerable<AppointmentListItemViewModel>> GetAllForListAsync();

        Task<AppointmentDetailsViewModel?> GetDetailsForDeleteAsync(int id);
    }
}