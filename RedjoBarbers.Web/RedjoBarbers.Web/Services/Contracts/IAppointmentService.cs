using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);

        Task<AppointmentFormViewModel?> GetFormModelByIdAsync(int id);

        Task<MyAppointmentsPageViewModel> GetMyAppointmentsPageAsync(string? phone);

        Task<AppointmentFormViewModel> GetCreateFormModelAsync();

        Task PopulateDropdownsAsync(AppointmentFormViewModel model);

        Task<bool> CreateAsync(AppointmentFormViewModel model);

        Task<bool> UpdateAsync(int id, AppointmentFormViewModel model);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> HasBusyTimeSlotAsync(DateTime targetDate, int? excludedAppointmentId, int? barberId);
    }
}