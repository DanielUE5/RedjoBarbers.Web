using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RedjoBarbers.Web.Controllers;
using RedjoBarbers.Web.Services.Contracts;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;
using RedjoBarbers.Web.ViewModels.Appointments;

namespace RedjoBarbers.Web.IntegrationTests.ControllerTests
{
    [TestFixture]
    public class AppointmentControllerTests
    {
        private Mock<IAppointmentService> appointmentServiceMock = null!;
        private AppointmentController controller = null!;

        [SetUp]
        public void Setup()
        {
            appointmentServiceMock = new Mock<IAppointmentService>();
            controller = new AppointmentController(appointmentServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithModel()
        {
            IEnumerable<AppointmentListItemViewModel> model = new List<AppointmentListItemViewModel>();

            appointmentServiceMock
                .Setup(x => x.GetAllForListAsync())
                .ReturnsAsync(model);

            IActionResult result = await controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task MyAppointments_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            IActionResult result = await controller.MyAppointments();

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task MyAppointments_ShouldReturnViewWithModel_WhenUserIdExists()
        {
            SetUser("user-1");

            MyAppointmentsPageViewModel model = new MyAppointmentsPageViewModel();

            appointmentServiceMock
                .Setup(x => x.GetMyAppointmentsPageAsync("user-1"))
                .ReturnsAsync(model);

            IActionResult result = await controller.MyAppointments();

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task CreateGet_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            IActionResult result = await controller.Create((int?)null);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task CreateGet_ShouldReturnViewWithModel_WhenServiceIdIsNotProvided()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.GetCreateFormModelAsync())
                .ReturnsAsync(model);

            IActionResult result = await controller.Create((int?)null);

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task CreateGet_ShouldPreselectService_WhenServiceIdIsProvided()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.GetCreateFormModelAsync())
                .ReturnsAsync(model);

            IActionResult result = await controller.Create(5);

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            AppointmentFormViewModel resultModel = (AppointmentFormViewModel)viewResult.Model!;

            Assert.That(resultModel.BarberServiceId, Is.EqualTo(5));
        }

        [Test]
        public async Task CreatePost_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            IActionResult result = await controller.Create(model);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task CreatePost_ShouldReturnView_WhenModelStateIsInvalid()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();
            controller.ModelState.AddModelError("Test", "Invalid");

            IActionResult result = await controller.Create(model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task CreatePost_ShouldReturnView_WhenBarberOrServiceIsInvalid()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, "user-1"))
                .ReturnsAsync(AppointmentCreateResult.InvalidBarberOrService);

            IActionResult result = await controller.Create(model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task CreatePost_ShouldReturnView_WhenBusySlot()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, "user-1"))
                .ReturnsAsync(AppointmentCreateResult.BusySlot);

            IActionResult result = await controller.Create(model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task CreatePost_ShouldRedirectToMyAppointments_WhenSuccess()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, "user-1"))
                .ReturnsAsync(AppointmentCreateResult.Success);

            IActionResult result = await controller.Create(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AppointmentController.MyAppointments)));
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
            SetUser("user-1");

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task UpdateGet_ShouldReturnNotFound_WhenModelDoesNotExist()
        {
            SetUser("user-1");

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.GetFormModelByIdAsync(1))
                .ReturnsAsync((AppointmentFormViewModel?)null);

            IActionResult result = await controller.Update(1);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateGet_ShouldReturnView_WhenModelExists()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.GetFormModelByIdAsync(1))
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

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnNotFound_WhenIdsDoNotMatch()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 2
            };

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnForbid_WhenUserCannotAccess()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnView_WhenModelStateIsInvalid()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            controller.ModelState.AddModelError("Test", "Invalid");

            IActionResult result = await controller.Update(1, model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task UpdatePost_ShouldReturnNotFound_WhenServiceReturnsNotFound()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.UpdateAsync(1, model))
                .ReturnsAsync(AppointmentUpdateResult.NotFound);

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnView_WhenServiceReturnsInvalidBarberOrService()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.UpdateAsync(1, model))
                .ReturnsAsync(AppointmentUpdateResult.InvalidBarberOrService);

            IActionResult result = await controller.Update(1, model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task UpdatePost_ShouldReturnView_WhenServiceReturnsBusySlot()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.UpdateAsync(1, model))
                .ReturnsAsync(AppointmentUpdateResult.BusySlot);

            IActionResult result = await controller.Update(1, model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task UpdatePost_ShouldRedirectToMyAppointments_WhenSuccess()
        {
            SetUser("user-1");

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.UpdateAsync(1, model))
                .ReturnsAsync(AppointmentUpdateResult.Success);

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AppointmentController.MyAppointments)));
        }

        [Test]
        public async Task DeleteGet_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task DeleteGet_ShouldReturnForbid_WhenUserCannotAccess()
        {
            SetUser("user-1");

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            SetUser("user-1");

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.GetDetailsForDeleteAsync(1))
                .ReturnsAsync((AppointmentDetailsViewModel?)null);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteGet_ShouldReturnView_WhenAppointmentExists()
        {
            SetUser("user-1");

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.GetDetailsForDeleteAsync(1))
                .ReturnsAsync(model);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<ViewResult>());

            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task DeletePost_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUser(null);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task DeletePost_ShouldReturnForbid_WhenUserCannotAccess()
        {
            SetUser("user-1");

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeletePost_ShouldReturnNotFound_WhenDeleteFails()
        {
            SetUser("user-1");

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeletePost_ShouldRedirectToMyAppointments_WhenDeleteSucceeds_AndUserIsNotAdmin()
        {
            SetUser("user-1");

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "user-1", false))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AppointmentController.MyAppointments)));
        }

        [Test]
        public async Task DeletePost_ShouldRedirectToIndex_WhenDeleteSucceeds_AndUserIsAdmin()
        {
            SetUser("admin-1", true);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, "admin-1", true))
                .ReturnsAsync(true);

            appointmentServiceMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AppointmentController.Index)));
        }

        [Test]
        public async Task GetAvailableSlots_ShouldReturnJsonResult_WithSlots()
        {
            SetUser("user-1");

            DateTime date = new DateTime(2026, 4, 10);
            int barberId = 2;
            int barberServiceId = 3;

            IEnumerable<string> slots = new List<string>
            {
                "10:00",
                "10:30",
                "11:00"
            };

            appointmentServiceMock
                .Setup(x => x.GetAvailableSlotsAsync(date, barberId, barberServiceId))
                .ReturnsAsync(slots);

            IActionResult result = await controller.GetAvailableSlots(date, barberId, barberServiceId);

            Assert.That(result, Is.TypeOf<JsonResult>());

            JsonResult jsonResult = (JsonResult)result;
            Assert.That(jsonResult.Value, Is.EqualTo(slots));
        }

        private void SetUser(string? userId, bool isAdmin = false)
        {
            List<Claim> claims = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

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