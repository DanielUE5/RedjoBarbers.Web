using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public ReviewController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? barberServiceId)
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

            // If a specific barber service ID is provided, filter the reviews to show only those for that service
            if (barberServiceId.HasValue && barberServiceId.Value > 0)
            {
                query = query.Where(r => r.BarberServiceId == barberServiceId.Value);
            }

            List<ReviewIndexItemViewModel> reviews = await query
                .ToListAsync();

            return View(reviews);
        }


        [HttpGet]
        public async Task<IActionResult> Create(int? barberServiceId)
        {
            IEnumerable<BarberService> services = await dbContext.BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Services = services;

            ReviewCreateViewModel model = new ReviewCreateViewModel
            {
                BarberServiceId = barberServiceId ?? 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var services = await dbContext.BarberServices
                    .AsNoTracking()
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                ViewBag.Services = services;

                return View(model);
            }

            Review review = new Review
            {
                BarberServiceId = model.BarberServiceId,
                CustomerName = model.CustomerName,
                Rating = model.Rating,
                Comments = model.Comments ?? "",
                ReviewDate = DateTime.Now
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index),
                new { barberServiceId = model.BarberServiceId });
        }

        // TODO: Implement Edit and Delete actions for reviews
        public IActionResult Delete()
        {
            return Ok("Works!");
        }

        public IActionResult Update()
        {
            return Ok("Works!");
        }
    }
}
