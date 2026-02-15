using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RedjoBarbersDbContext dbContext;

        public AdminController(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Appointment> appointments = await dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Barber)
                .Include(a => a.BarberService)
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            List<Review> reviews = await dbContext.Reviews
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            AdminPanelViewModel vm = new AdminPanelViewModel
            {
                Appointments = appointments,
                Reviews = reviews
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
        {
            Appointment? appointment = await dbContext.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = status;
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            Review? review = await dbContext.Reviews.FindAsync(id);

            if (review != null)
            {
                dbContext.Reviews.Remove(review);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}