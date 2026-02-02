using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Controllers
{
    public class BarberServicesController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public BarberServicesController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            List<BarberService> allServices = await dbContext
                .BarberServices
                .Include(bs => bs.Reviews)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return View(allServices);
        }
    }
}
