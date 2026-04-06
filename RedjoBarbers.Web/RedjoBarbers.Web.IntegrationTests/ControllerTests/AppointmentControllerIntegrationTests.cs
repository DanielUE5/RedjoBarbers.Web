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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            MyAppointmentsPageViewModel model = new MyAppointmentsPageViewModel();

            appointmentServiceMock
                .Setup(x => x.GetMyAppointmentsPageAsync(userId))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, userId))
                .ReturnsAsync(AppointmentCreateResult.InvalidBarberOrService);

            IActionResult result = await controller.Create(model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task CreatePost_ShouldReturnView_WhenBusySlot()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, userId))
                .ReturnsAsync(AppointmentCreateResult.BusySlot);

            IActionResult result = await controller.Create(model);

            appointmentServiceMock.Verify(x => x.PopulateDropdownsAsync(model), Times.Once);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task CreatePost_ShouldRedirectToMyAppointments_WhenSuccess()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.CreateAsync(model, userId))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            appointmentServiceMock
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

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel();

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Update(1, model);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task UpdatePost_ShouldReturnView_WhenModelStateIsInvalid()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(1);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
                .ReturnsAsync(false);

            IActionResult result = await controller.Delete(model);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeletePost_ShouldReturnNotFound_WhenDeleteFails()
        {
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, userId, false))
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
            Guid adminUserId = Guid.NewGuid();
            SetUser(adminUserId, true);

            AppointmentDetailsViewModel model = new AppointmentDetailsViewModel
            {
                Id = 1
            };

            appointmentServiceMock
                .Setup(x => x.IsOwnerOrAdminAsync(1, adminUserId, true))
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
            Guid userId = Guid.NewGuid();
            SetUser(userId);

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

        private void SetUser(Guid? userId, bool isAdmin = false)
        {
            List<Claim> claims = new List<Claim>();

            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
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