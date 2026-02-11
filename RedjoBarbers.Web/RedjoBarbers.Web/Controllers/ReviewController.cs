using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public ReviewController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int barberServiceId)
        {
            IEnumerable<Review> reviews = await dbContext
                .Reviews
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }

        public IActionResult Create()
        {
            return Ok("Works!");
        }

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
