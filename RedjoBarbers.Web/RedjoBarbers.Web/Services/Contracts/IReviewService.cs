using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IReviewService
    {
        Task<ReviewIndexPageViewModel> GetAllAsync(int? barberServiceId, string? sortReviews);

        Task<IEnumerable<BarberService>> GetAllServicesAsync();

        Task<ReviewCreateViewModel> GetCreateModelAsync(int? barberServiceId);

        Task<bool> CreateAsync(ReviewCreateViewModel model, string userId);

        Task<ReviewUpdateViewModel?> GetUpdateModelAsync(int id);

        Task<bool> UpdateAsync(ReviewUpdateViewModel model);

        Task<bool> DeleteAsync(int id);

        Task<bool> IsOwnerAsync(int reviewId, string userId);

        Task<bool> IsOwnerOrAdminAsync(int reviewId, string userId, bool isAdmin);

        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
    }
}