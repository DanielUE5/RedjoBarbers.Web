using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;
using System.Diagnostics;

namespace RedjoBarbers.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService homeService)
        {
            this.homeService = homeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel vm = await homeService.GetHomePageDataAsync();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            Barber? owner = await homeService.GetOwnerAsync();
            return View(owner);
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            IEnumerable<Barber> contacts = await homeService.GetContactsAsync();
            return View(contacts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        [Route("Home/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404)
            {
                return View("Error404");
            }

            if (statusCode == 500)
            {
                return View("Error500");
            }

            return View("Error");
        }
    }
}
