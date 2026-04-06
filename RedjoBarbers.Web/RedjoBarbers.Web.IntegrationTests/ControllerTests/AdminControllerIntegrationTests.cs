using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RedjoBarbers.Web.Areas.Admin.Controllers;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.IntegrationTests.ControllerTests;

[TestFixture]
public class AdminControllerTests
{
    private RedjoBarbersDbContext dbContext = null!;
    private AdminController controller = null!;

    [SetUp]
    public void Setup()
    {
        DbContextOptions<RedjoBarbersDbContext> options = new DbContextOptionsBuilder<RedjoBarbersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new RedjoBarbersDbContext(options);
        controller = new AdminController(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        controller.Dispose();
        dbContext.Dispose();
    }

    [Test]
    public async Task Index_ShouldReturnViewWithAdminPanelViewModel()
    {
        Guid userId = Guid.NewGuid();

        Barber barber = new Barber
        {
            Id = 1,
            Name = "Ivan Petrov",
            Bio = "Senior barber with experience.",
            PhotoUrl = "https://example.com/barber.jpg",
            PhoneNumber = "+359888123456"
        };

        BarberService barberService = new BarberService
        {
            Id = 1,
            Name = "Haircut",
            Description = "Standard haircut service",
            Price = 20,
            IsActive = true
        };

        Appointment appointment = new Appointment
        {
            Id = 1,
            AppointmentDate = DateTime.UtcNow,
            CustomerName = "Daniel",
            CustomerEmail = "daniel@test.com",
            CustomerPhone = "+359888000111",
            Notes = "Test note",
            Status = AppointmentStatus.Pending,
            BarberId = barber.Id,
            Barber = barber,
            BarberServiceId = barberService.Id,
            BarberService = barberService,
            UserId = userId
        };

        Review review = new Review
        {
            Id = 1,
            CustomerName = "Daniel",
            Rating = 5,
            Comments = "Very good service",
            ReviewDate = DateTime.UtcNow,
            BarberServiceId = barberService.Id,
            BarberService = barberService,
            UserId = userId
        };

        await dbContext.Barbers.AddAsync(barber);
        await dbContext.BarberServices.AddAsync(barberService);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.Reviews.AddAsync(review);
        await dbContext.SaveChangesAsync();

        IActionResult result = await controller.Index();

        Assert.That(result, Is.TypeOf<ViewResult>());

        ViewResult viewResult = (ViewResult)result;
        Assert.That(viewResult.Model, Is.TypeOf<AdminPanelViewModel>());

        AdminPanelViewModel model = (AdminPanelViewModel)viewResult.Model!;
        Assert.That(model.Appointments.Count(), Is.EqualTo(1));
        Assert.That(model.Reviews.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateStatus_ShouldRedirectToIndex_WhenAppointmentDoesNotExist()
    {
        IActionResult result = await controller.UpdateStatus(999, AppointmentStatus.Confirmed);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        RedirectToActionResult redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task UpdateStatus_ShouldUpdateStatusAndRedirect_WhenAppointmentExists()
    {
        Guid userId = Guid.NewGuid();

        Barber barber = new Barber
        {
            Id = 1,
            Name = "Ivan Petrov",
            Bio = "Senior barber with experience.",
            PhotoUrl = "https://example.com/barber.jpg",
            PhoneNumber = "+359888123456"
        };

        BarberService barberService = new BarberService
        {
            Id = 1,
            Name = "Haircut",
            Description = "Standard haircut service",
            Price = 20,
            IsActive = true
        };

        Appointment appointment = new Appointment
        {
            Id = 1,
            AppointmentDate = DateTime.UtcNow,
            CustomerName = "Daniel",
            CustomerEmail = "daniel@test.com",
            CustomerPhone = "+359888000111",
            Status = AppointmentStatus.Pending,
            BarberId = barber.Id,
            Barber = barber,
            BarberServiceId = barberService.Id,
            BarberService = barberService,
            UserId = userId
        };

        await dbContext.Barbers.AddAsync(barber);
        await dbContext.BarberServices.AddAsync(barberService);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();

        IActionResult result = await controller.UpdateStatus(1, AppointmentStatus.Confirmed);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        Appointment? updatedAppointment = await dbContext.Appointments.FindAsync(1);
        Assert.That(updatedAppointment, Is.Not.Null);
        Assert.That(updatedAppointment!.Status, Is.EqualTo(AppointmentStatus.Confirmed));

        RedirectToActionResult redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task DeleteReview_ShouldRemoveReviewAndRedirect_WhenReviewExists()
    {
        Guid userId = Guid.NewGuid();

        BarberService barberService = new BarberService
        {
            Id = 1,
            Name = "Haircut",
            Description = "Standard haircut service",
            Price = 20,
            IsActive = true
        };

        Review review = new Review
        {
            Id = 1,
            CustomerName = "Daniel",
            Rating = 5,
            Comments = "Nice service",
            ReviewDate = DateTime.UtcNow,
            BarberServiceId = barberService.Id,
            BarberService = barberService,
            UserId = userId
        };

        await dbContext.BarberServices.AddAsync(barberService);
        await dbContext.Reviews.AddAsync(review);
        await dbContext.SaveChangesAsync();

        IActionResult result = await controller.DeleteReview(1);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        Review? deletedReview = await dbContext.Reviews.FindAsync(1);
        Assert.That(deletedReview, Is.Null);

        RedirectToActionResult redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task DeleteReview_ShouldRedirect_WhenReviewDoesNotExist()
    {
        IActionResult result = await controller.DeleteReview(999);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        RedirectToActionResult redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
    }
}