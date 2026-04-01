using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services
{
    public class ReviewService : IReviewService
    {
        private readonly RedjoBarbersDbContext dbContext;

        public ReviewService(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ReviewIndexPageViewModel> GetAllAsync(int? barberServiceId, string? sortReviews)
        {
            IQueryable<Review> sortQuery = dbContext.Reviews
                .AsNoTracking();

            if (barberServiceId.HasValue && barberServiceId.Value > 0)
            {
                sortQuery = sortQuery.Where(r => r.BarberServiceId == barberServiceId.Value);
            }

            sortQuery = sortReviews switch
            {
                "rating" => sortQuery
                    .OrderByDescending(r => r.Rating)
                    .ThenByDescending(r => r.ReviewDate),

                _ => sortQuery
                    .OrderByDescending(r => r.ReviewDate)
            };

            List<ReviewIndexItemViewModel> reviews = await sortQuery
                .Select(r => new ReviewIndexItemViewModel
                {
                    Id = r.Id,
                    BarberServiceId = r.BarberServiceId,
                    CustomerName = r.CustomerName,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate,
                    Comments = r.Comments,
                    ServiceName = r.BarberService.Name
                })
                .ToListAsync();

            List<SelectListItem> services = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            return new ReviewIndexPageViewModel
            {
                BarberServiceId = barberServiceId,
                SortReviews = sortReviews,
                Services = services,
                Reviews = reviews
            };
        }

        public async Task<IEnumerable<BarberService>> GetAllServicesAsync()
        {
            return await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public Task<ReviewCreateViewModel> GetCreateModelAsync(int? barberServiceId)
        {
            return Task.FromResult(new ReviewCreateViewModel
            {
                BarberServiceId = barberServiceId ?? 0
            });
        }

        public async Task<bool> CreateAsync(ReviewCreateViewModel model, string userId)
        {
            Review review = new Review
            {
                BarberServiceId = model.BarberServiceId,
                CustomerName = model.CustomerName,
                Rating = model.Rating,
                Comments = model.Comments ?? string.Empty,
                ReviewDate = DateTime.Now,
                UserId = userId
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsOwnerAsync(int reviewId, string userId)
        {
            return await dbContext.Reviews
                .AsNoTracking()
                .AnyAsync(r => r.Id == reviewId && r.UserId == userId);
        }

        public async Task<bool> IsOwnerOrAdminAsync(int reviewId, string userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await dbContext.Reviews
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == reviewId);
            }

            return await dbContext.Reviews
                .AsNoTracking()
                .AnyAsync(r => r.Id == reviewId && r.UserId == userId);
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
        {
            return await dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.BarberService)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<ReviewUpdateViewModel?> GetUpdateModelAsync(int id)
        {
            Review? review = await dbContext.Reviews.FindAsync(id);

            if (review == null)
            {
                return null;
            }

            return new ReviewUpdateViewModel
            {
                Id = review.Id,
                BarberServiceId = review.BarberServiceId,
                CustomerName = review.CustomerName,
                Rating = review.Rating,
                Comments = review.Comments
            };
        }

        public async Task<bool> UpdateAsync(ReviewUpdateViewModel model)
        {
            Review? review = await dbContext.Reviews.FindAsync(model.Id);

            if (review == null)
            {
                return false;
            }

            review.CustomerName = model.CustomerName;
            review.Rating = model.Rating;
            review.Comments = model.Comments ?? string.Empty;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Review? review = await dbContext.Reviews.FindAsync(id);

            if (review == null)
            {
                return false;
            }

            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}