using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RedjoBarbers.Web.Controllers;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.IntegrationTests.Tests.ControllerTests
{
    [TestFixture]
    public class ReviewControllerTests
    {
        private Mock<IReviewService> reviewServiceMock = null!;
        private ReviewController controller = null!;

        [SetUp]
        public void Setup()
        {
            reviewServiceMock = new Mock<IReviewService>();
            controller = new ReviewController(reviewServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithModel()
        {
            ReviewIndexPageViewModel model = new();

            reviewServiceMock
                .Setup(x => x.GetAllAsync(1, "newest", 2))
                .ReturnsAsync(model);

            IActionResult result = await controller.Index(1, "newest", 2);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task CreateGet_ShouldReturnViewWithModelAndServices()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            IEnumerable<BarberService> services = new List<BarberService>
            {
                new BarberService(),
                new BarberService()
            };

            ReviewCreateViewModel model = new();

            reviewServiceMock
                .Setup(x => x.GetAllServicesAsync())
                .ReturnsAsync(services);

            reviewServiceMock
                .Setup(x => x.GetCreateModelAsync(3))
                .ReturnsAsync(model);

            IActionResult result = await controller.Create(3);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(controller.ViewBag.Services, Is.EqualTo(services));
        }

        [Test]
        public async Task CreatePost_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);
            ReviewCreateViewModel model = new();

            IActionResult result = await controller.Create(model);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task CreatePost_ShouldReturnView_WhenModelStateIsInvalid()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewCreateViewModel model = new();

            IEnumerable<BarberService> services = new List<BarberService>
            {
                new BarberService(),
                new BarberService()
            };

            controller.ModelState.AddModelError("Test", "Invalid");

            reviewServiceMock
                .Setup(x => x.GetAllServicesAsync())
                .ReturnsAsync(services);

            IActionResult result = await controller.Create(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(controller.ViewBag.Services, Is.EqualTo(services));
        }

        [Test]
        public async Task CreatePost_ShouldRedirectToMyAppointments_WhenModelStateIsValid()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewCreateViewModel model = new();

            IActionResult result = await controller.Create(model);

            reviewServiceMock.Verify(x => x.CreateAsync(model, userId), Times.Once);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("MyAppointments"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Appointment"));
        }

        [Test]
        public async Task Delete_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Delete_ShouldReturnForbid_WhenUserCannotAccess()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_ShouldRedirectToMyAppointments_WhenDeleteSucceeds()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("MyAppointments"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Appointment"));
        }

        [Test]
        public async Task UpdateGet_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdateGet_ShouldReturnForbid_WhenUserCannotAccess()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task UpdateGet_ShouldReturnNotFound_WhenModelDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.GetUpdateModelAsync(1))
                .ReturnsAsync((ReviewUpdateViewModel?)null);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateGet_ShouldReturnView_WhenModelExists()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewUpdateViewModel model = new() { Id = 1 };

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.GetUpdateModelAsync(1))
                .ReturnsAsync(model);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task UpdatePost_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);
            ReviewUpdateViewModel model = new() { Id = 1 };

            IActionResult result = await controller.Update(model);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnForbid_WhenUserCannotAccess()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewUpdateViewModel model = new() { Id = 1 };

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(model);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnView_WhenModelStateIsInvalid()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewUpdateViewModel model = new() { Id = 1 };

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            controller.ModelState.AddModelError("Test", "Invalid");

            IActionResult result = await controller.Update(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task UpdatePost_ShouldReturnNotFound_WhenUpdateFails()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewUpdateViewModel model = new() { Id = 1 };

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.UpdateAsync(model))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(model);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldRedirectToMyAppointments_WhenUpdateSucceeds()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            ReviewUpdateViewModel model = new() { Id = 1 };

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.UpdateAsync(model))
                .ReturnsAsync(true);

            IActionResult result = await controller.Update(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("MyAppointments"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Appointment"));
        }

        [Test]
        public async Task Delete_ShouldAllowAdminAccess_WhenUserIsAdmin()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId, true);

            reviewServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, true))
                .ReturnsAsync(true);

            reviewServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("MyAppointments"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Appointment"));
        }

        private void SetUser(Guid? userId, bool isAdmin = false)
        {
            List<Claim> claims = new();

            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            ClaimsIdentity identity = new(claims, "TestAuthType");
            ClaimsPrincipal principal = new(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }
    }
}