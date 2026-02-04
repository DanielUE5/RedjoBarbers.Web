using Microsoft.AspNetCore.Mvc;

namespace RedjoBarbers.Web.Controllers
{
    public class BarberController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Works!");
        }
    }
}
