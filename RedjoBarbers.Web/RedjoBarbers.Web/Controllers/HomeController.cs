using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;
        public HomeController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
                .ThenBy(b => b.Name)
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
