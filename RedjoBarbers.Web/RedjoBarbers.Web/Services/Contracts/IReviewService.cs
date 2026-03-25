using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewIndexItemViewModel>> GetAllAsync(int? barberServiceId);

        Task<IEnumerable<BarberService>> GetAllServicesAsync();

        Task<ReviewCreateViewModel> GetCreateModelAsync(int? barberServiceId);

        Task<bool> CreateAsync(ReviewCreateViewModel model);

        Task<ReviewUpdateViewModel?> GetUpdateModelAsync(int id);

        Task<bool> UpdateAsync(ReviewUpdateViewModel model);

        Task<bool> DeleteAsync(int id);
    }
}