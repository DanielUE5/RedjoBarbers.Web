using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;

namespace RedjoBarbers.Web.Controllers
{
    [AllowAnonymous]
    public class BarberServicesController : Controller
    {
        private readonly IBarberServiceService barberServiceService;

        public BarberServicesController(IBarberServiceService barberServiceService)
        {
            this.barberServiceService = barberServiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<BarberService> allServices = await barberServiceService.GetAllAsync();
            return View(allServices);
        }
    }
}