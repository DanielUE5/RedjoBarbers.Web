using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Data.Models.Enums;
using RedjoBarbers.Web.Services;
using RedjoBarbers.Web.Services.Results;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Tests.ServiceTests
{
    [TestFixture]
    public class AppointmentServiceTests
    {
        private RedjoBarbersDbContext context = null!;
        private AppointmentService appointmentService = null!;

        [SetUp]
        public async Task SetUp()
        {
            DbContextOptions<RedjoBarbersDbContext> options =
                new DbContextOptionsBuilder<RedjoBarbersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new RedjoBarbersDbContext(options);

            Guid user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Guid user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            context.Barbers.AddRange(
                new Barber
                {
                    Id = 1,
                    Name = "Реджеп",
                    PhoneNumber = "+359888111111",
                    Bio = "Owner barber",
                    PhotoUrl = "owner.jpg"
                },
                new Barber
                {
                    Id = 2,
                    Name = "Ivan",
                    PhoneNumber = "+359888222222",
                    Bio = "Second barber",
                    PhotoUrl = "ivan.jpg"
                });

            context.BarberServices.AddRange(
                new BarberService
                {
                    Id = 1,
                    Name = "Haircut",
                    Description = "Basic haircut",
                    Price = 20,
                    IsActive = true,
                    DurationMinutes = 30
                },
                new BarberService
                {
                    Id = 2,
                    Name = "Beard Trim",
                    Description = "Beard trimming",
                    Price = 15,
                    IsActive = true,
                    DurationMinutes = 45
                });

            context.Appointments.AddRange(
                new Appointment
                {
                    Id = 1,
                    AppointmentDate = new DateTime(2026, 4, 10, 10, 0, 0),
                    CustomerName = "Daniel",
                    CustomerEmail = "daniel@test.com",
                    CustomerPhone = "+359888333333",
                    Notes = "Existing appointment",
                    Status = AppointmentStatus.Pending,
                    BarberId = 1,
                    BarberServiceId = 1,
                    UserId = user1Id,
                    DurationMinutes = 30
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDate = new DateTime(2026, 4, 11, 12, 0, 0),
                    CustomerName = "Peter",
                    CustomerEmail = "peter@test.com",
                    CustomerPhone = "+359888444444",
                    Notes = "Second appointment",
                    Status = AppointmentStatus.Confirmed,
                    BarberId = 2,
                    BarberServiceId = 2,
                    UserId = user2Id,
                    DurationMinutes = 45
                },
                new Appointment
                {
                    Id = 3,
                    AppointmentDate = new DateTime(2026, 4, 12, 15, 0, 0),
                    CustomerName = "Maria",
                    CustomerEmail = "maria@test.com",
                    CustomerPhone = "+359888555555",
                    Notes = "Cancelled appointment",
                    Status = AppointmentStatus.Cancelled,
                    BarberId = 1,
                    BarberServiceId = 2,
                    UserId = user1Id,
                    DurationMinutes = 45
                });

            context.Reviews.AddRange(
                new Review
                {
                    Id = 1,
                    BarberServiceId = 1,
                    CustomerName = "Daniel",
                    Rating = 5,
                    Comments = "Excellent",
                    ReviewDate = new DateTime(2026, 4, 1, 10, 0, 0),
                    UserId = user1Id
                },
                new Review
                {
                    Id = 2,
                    BarberServiceId = 2,
                    CustomerName = "Peter",
                    Rating = 4,
                    Comments = "Very good",
                    ReviewDate = new DateTime(2026, 4, 2, 10, 0, 0),
                    UserId = user2Id
                });

            await context.SaveChangesAsync();

            appointmentService = new AppointmentService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAppointmentsOrderedByDate()
        {
            IEnumerable<Appointment> appointments = await appointmentService.GetAllAsync();
            List<Appointment> result = appointments.ToList();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[2].Id, Is.EqualTo(3));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAppointment_WhenAppointmentExists()
        {
            Appointment? result = await appointmentService.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.CustomerName, Is.EqualTo("Daniel"));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAppointmentDoesNotExist()
        {
            Appointment? result = await appointmentService.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCreateFormModelAsync_ShouldReturnModelWithDropdowns()
        {
            AppointmentFormViewModel result = await appointmentService.GetCreateFormModelAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Barbers.Count(), Is.EqualTo(2));
            Assert.That(result.BarberServices.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetFormModelByIdAsync_ShouldReturnModel_WhenAppointmentExists()
        {
            AppointmentFormViewModel? result = await appointmentService.GetFormModelByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.CustomerName, Is.EqualTo("Daniel"));
            Assert.That(result.BarberId, Is.EqualTo(1));
            Assert.That(result.BarberServiceId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetFormModelByIdAsync_ShouldReturnNull_WhenAppointmentDoesNotExist()
        {
            AppointmentFormViewModel? result = await appointmentService.GetFormModelByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMyAppointmentsPageAsync_ShouldReturnOnlyCurrentUsersAppointmentsAndReviews()
        {
            Guid user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

            MyAppointmentsPageViewModel result = await appointmentService.GetMyAppointmentsPageAsync(user1Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Appointments.Count(), Is.EqualTo(2));
            Assert.That(result.Reviews.Count(), Is.EqualTo(1));
            Assert.That(result.Appointments.All(a => a.UserId == user1Id), Is.True);
            Assert.That(result.Reviews.All(r => r.UserId == user1Id), Is.True);
        }

        [Test]
        public async Task PopulateDropdownsAsync_ShouldPopulateBarbersAndServices()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel();

            await appointmentService.PopulateDropdownsAsync(model);

            Assert.That(model.Barbers.Count(), Is.EqualTo(2));
            Assert.That(model.BarberServices.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task CreateAsync_ShouldReturnInvalidBarberOrService_WhenBarberDoesNotExist()
        {
            Guid user3Id = Guid.NewGuid();

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 20, 10, 0, 0),
                CustomerName = "Invalid",
                CustomerEmail = "invalid@test.com",
                CustomerPhone = "+359888666666",
                Notes = "Invalid barber",
                BarberId = 999,
                BarberServiceId = 1
            };

            AppointmentCreateResult result = await appointmentService.CreateAsync(model, user3Id);

            Assert.That(result, Is.EqualTo(AppointmentCreateResult.InvalidBarberOrService));
        }

        [Test]
        public async Task CreateAsync_ShouldReturnBusySlot_WhenAppointmentTimeIsAlreadyTaken()
        {
            Guid user3Id = Guid.NewGuid();

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 10, 10, 15, 0),
                CustomerName = "Busy User",
                CustomerEmail = "busy@test.com",
                CustomerPhone = "+359888777777",
                Notes = "Busy slot",
                BarberId = 1,
                BarberServiceId = 1
            };

            AppointmentCreateResult result = await appointmentService.CreateAsync(model, user3Id);

            Assert.That(result, Is.EqualTo(AppointmentCreateResult.BusySlot));
            Assert.That(context.Appointments.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task CreateAsync_ShouldCreateAppointment_WhenDataIsValid()
        {
            Guid user3Id = Guid.NewGuid();

            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 15, 14, 0, 0),
                CustomerName = "New User",
                CustomerEmail = "new@test.com",
                CustomerPhone = "+359888888888",
                Notes = "Valid appointment",
                BarberId = 1,
                BarberServiceId = 1
            };

            AppointmentCreateResult result = await appointmentService.CreateAsync(model, user3Id);

            Assert.That(result, Is.EqualTo(AppointmentCreateResult.Success));
            Assert.That(context.Appointments.Count(), Is.EqualTo(4));

            Appointment? createdAppointment = context.Appointments
                .FirstOrDefault(a => a.UserId == user3Id);

            Assert.That(createdAppointment, Is.Not.Null);
            Assert.That(createdAppointment!.Status, Is.EqualTo(AppointmentStatus.Pending));
            Assert.That(createdAppointment.DurationMinutes, Is.EqualTo(30));
        }

        [Test]
        public async Task IsOwnerAsync_ShouldReturnTrue_WhenUserOwnsAppointment()
        {
            Guid user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

            bool result = await appointmentService.IsOwnerAsync(1, user1Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsOwnerAsync_ShouldReturnFalse_WhenUserDoesNotOwnAppointment()
        {
            Guid differentUserId = Guid.NewGuid();

            bool result = await appointmentService.IsOwnerAsync(1, differentUserId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsOwnerOrAdminAsync_ShouldReturnTrue_WhenUserIsAdmin()
        {
            Guid differentUserId = Guid.NewGuid();

            bool result = await appointmentService.IsOwnerOrAdminAsync(1, differentUserId, true);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsOwnerOrAdminAsync_ShouldReturnFalse_WhenUserIsNotOwnerAndNotAdmin()
        {
            Guid differentUserId = Guid.NewGuid();

            bool result = await appointmentService.IsOwnerOrAdminAsync(1, differentUserId, false);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 20, 10, 0, 0),
                CustomerName = "Missing",
                CustomerEmail = "missing@test.com",
                CustomerPhone = "+359888999999",
                Notes = "Missing appointment",
                BarberId = 1,
                BarberServiceId = 1
            };

            AppointmentUpdateResult result = await appointmentService.UpdateAsync(999, model);

            Assert.That(result, Is.EqualTo(AppointmentUpdateResult.NotFound));
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnBusySlot_WhenAnotherAppointmentConflicts()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 10, 10, 15, 0),
                CustomerName = "Peter Updated",
                CustomerEmail = "peter@test.com",
                CustomerPhone = "+359888444444",
                Notes = "Conflict update",
                BarberId = 1,
                BarberServiceId = 1
            };

            AppointmentUpdateResult result = await appointmentService.UpdateAsync(2, model);

            Assert.That(result, Is.EqualTo(AppointmentUpdateResult.BusySlot));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateAppointment_WhenDataIsValid()
        {
            AppointmentFormViewModel model = new AppointmentFormViewModel
            {
                AppointmentDate = new DateTime(2026, 4, 20, 16, 0, 0),
                CustomerName = "Daniel Updated",
                CustomerEmail = "daniel.updated@test.com",
                CustomerPhone = "+359888000000",
                Notes = "Updated appointment",
                BarberId = 2,
                BarberServiceId = 2
            };

            AppointmentUpdateResult result = await appointmentService.UpdateAsync(1, model);

            Assert.That(result, Is.EqualTo(AppointmentUpdateResult.Success));

            Appointment? updatedAppointment = await context.Appointments.FindAsync(1);

            Assert.That(updatedAppointment, Is.Not.Null);
            Assert.That(updatedAppointment!.CustomerName, Is.EqualTo("Daniel Updated"));
            Assert.That(updatedAppointment.BarberId, Is.EqualTo(2));
            Assert.That(updatedAppointment.BarberServiceId, Is.EqualTo(2));
            Assert.That(updatedAppointment.DurationMinutes, Is.EqualTo(45));
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenAppointmentDoesNotExist()
        {
            bool result = await appointmentService.DeleteAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveAppointment_WhenAppointmentExists()
        {
            bool result = await appointmentService.DeleteAsync(1);

            Assert.That(result, Is.True);
            Assert.That(await context.Appointments.FindAsync(1), Is.Null);
            Assert.That(context.Appointments.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenAppointmentExists()
        {
            bool result = await appointmentService.ExistsAsync(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenAppointmentDoesNotExist()
        {
            bool result = await appointmentService.ExistsAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasBusyTimeSlotAsync_ShouldReturnFalse_WhenOnlyCancelledAppointmentExists()
        {
            bool result = await appointmentService.HasBusyTimeSlotAsync(
                new DateTime(2026, 4, 12, 15, 15, 0),
                30,
                null,
                1);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasBusyTimeSlotAsync_ShouldReturnTrue_WhenNonCancelledAppointmentExistsInWindow()
        {
            bool result = await appointmentService.HasBusyTimeSlotAsync(
                new DateTime(2026, 4, 10, 10, 20, 0),
                30,
                null,
                1);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetFilteredAsync_ShouldFilterByStatus()
        {
            AppointmentFilterViewModel model = new AppointmentFilterViewModel
            {
                Status = AppointmentStatus.Pending,
                Page = 1,
                PageSize = 8
            };

            AppointmentFilterViewModel result = await appointmentService.GetFilteredAsync(model);

            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Appointments.Count(), Is.EqualTo(1));
            Assert.That(result.Appointments.All(a => a.Status == AppointmentStatus.Pending), Is.True);
        }

        [Test]
        public async Task GetFilteredAsync_ShouldSetDefaultPageSize_WhenPageSizeIsZero()
        {
            AppointmentFilterViewModel model = new AppointmentFilterViewModel
            {
                Page = 1,
                PageSize = 0
            };

            AppointmentFilterViewModel result = await appointmentService.GetFilteredAsync(model);

            Assert.That(result.PageSize, Is.EqualTo(8));
        }

        [Test]
        public async Task GetFilteredAsync_ShouldCapPageSizeAtThirty_WhenPageSizeIsTooLarge()
        {
            AppointmentFilterViewModel model = new AppointmentFilterViewModel
            {
                Page = 1,
                PageSize = 100
            };

            AppointmentFilterViewModel result = await appointmentService.GetFilteredAsync(model);

            Assert.That(result.PageSize, Is.EqualTo(30));
        }
    }
}