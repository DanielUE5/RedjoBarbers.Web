using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Controllers
{
    [AllowAnonymous]
    public class BarberServicesController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public BarberServicesController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<BarberService> allServices = await dbContext
                .BarberServices
                .OrderBy(bs => bs.Id)
                .Include(bs => bs.Reviews)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return View(allServices);
        }
    }
}
