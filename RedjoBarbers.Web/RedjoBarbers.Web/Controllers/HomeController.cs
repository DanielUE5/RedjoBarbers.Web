using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public HomeController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> Index()
        {
            List<BarberService> services = await dbContext
                .BarberServices
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new BarberService
                {
                    Name = s.Name,
                    Description = s.Description
                })
                .ToListAsync();

            List<Review> reviews = await dbContext
                .Reviews
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate)
                .ThenBy(r => r.Rating)
                .Where(r => r.Rating == 5 &&
                            !string.IsNullOrWhiteSpace(r.Comments))
                .Take(4)
                .Select(r => new Review
                {
                    BarberServiceId = r.BarberServiceId,
                    CustomerName = r.CustomerName,
                    Comments = r.Comments,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate,
                })
                .ToListAsync();

            HomeIndexViewModel vm = new HomeIndexViewModel
            {
                Services = services,
                Reviews = reviews
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            Barber? owner = await dbContext
                .Barbers
                .AsNoTracking()
                .Where(b => b.Name.Contains("Реджеп"))
                .Select(b => new Barber
                {
                    Name = b.Name,
                    PhoneNumber = b.PhoneNumber,
                })
                .SingleOrDefaultAsync();

            return View(owner);
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            IEnumerable<Barber> contacts = await dbContext
                .Barbers
                .OrderBy(b => b.Id)
                .AsNoTracking()
                .ToListAsync();

            return View(contacts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
