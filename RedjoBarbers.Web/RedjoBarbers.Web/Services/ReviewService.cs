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

        public async Task<IEnumerable<ReviewIndexItemViewModel>> GetAllAsync(int? barberServiceId)
        {
            IQueryable<ReviewIndexItemViewModel> query = dbContext.Reviews
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewIndexItemViewModel
                {
                    Id = r.Id,
                    BarberServiceId = r.BarberServiceId,
                    CustomerName = r.CustomerName,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate,
                    Comments = r.Comments,
                    ServiceName = r.BarberService.Name
                });

            if (barberServiceId.HasValue && barberServiceId.Value > 0)
            {
                query = query.Where(r => r.BarberServiceId == barberServiceId.Value);
            }

            return await query.ToListAsync();
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

        public async Task<bool> CreateAsync(ReviewCreateViewModel model)
        {
            Review review = new Review
            {
                BarberServiceId = model.BarberServiceId,
                CustomerName = model.CustomerName,
                Rating = model.Rating,
                Comments = model.Comments ?? string.Empty,
                ReviewDate = DateTime.Now
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            return true;
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