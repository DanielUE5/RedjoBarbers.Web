using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);

        Task<AppointmentFormViewModel?> GetFormModelByIdAsync(int id);

        Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(string userId);

        Task<AppointmentFormViewModel> GetCreateFormModelAsync();

        Task PopulateDropdownsAsync(AppointmentFormViewModel model);

        Task<AppointmentCreateResult> CreateAsync(AppointmentFormViewModel model, string userId);

        Task<bool> IsOwnerAsync(int appointmentId, string userId);

        Task<bool> IsOwnerOrAdminAsync(int appointmentId, string userId, bool isAdmin);

        Task<AppointmentUpdateResult> UpdateAsync(int id, AppointmentFormViewModel model);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> HasBusyTimeSlotAsync(DateTime targetDate, int? excludedAppointmentId, int? barberId);
    }
}