using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services
{
    public class HomeService : IHomeService
    {
        private readonly RedjoBarbersDbContext dbContext;

        public HomeService(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<HomeIndexViewModel> GetHomePageDataAsync()
        {
            List<BarberService> services = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new BarberService
                {
                    Name = s.Name,
                    Description = s.Description
                })
                .ToListAsync();

            List<Review> reviews = await dbContext.Reviews
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate)
                .ThenBy(r => r.Rating)
                .Where(r => r.Rating == 5 && !string.IsNullOrWhiteSpace(r.Comments))
                .Take(4)
                .Select(r => new Review
                {
                    BarberServiceId = r.BarberServiceId,
                    CustomerName = r.CustomerName,
                    Comments = r.Comments,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return new HomeIndexViewModel
            {
                Services = services,
                Reviews = reviews
            };
        }

        public async Task<Barber?> GetOwnerAsync()
        {
            return await dbContext.Barbers
                .AsNoTracking()
                .Where(b => b.Name.Contains("Реджеп"))
                .Select(b => new Barber
                {
                    Name = b.Name,
                    PhoneNumber = b.PhoneNumber
                })
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Barber>> GetContactsAsync()
        {
            return await dbContext.Barbers
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .ToListAsync();
        }
    }
}