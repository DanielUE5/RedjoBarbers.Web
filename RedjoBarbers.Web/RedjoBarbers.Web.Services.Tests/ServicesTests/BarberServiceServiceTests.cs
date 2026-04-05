using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Services.Tests.ServiceTests
{
    [TestFixture]
    public class BarberServiceServiceTests
    {
        private RedjoBarbersDbContext context;
        private BarberServiceService barberServiceService;

        [SetUp]
        public async Task SetUp()
        {
            DbContextOptions<RedjoBarbersDbContext> options =
                new DbContextOptionsBuilder<RedjoBarbersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new RedjoBarbersDbContext(options);

            context.BarberServices.AddRange(
                new BarberService
                {
                    Id = 1,
                    Name = "Haircut",
                    Description = "Basic haircut service",
                    Price = 20,
                    IsActive = true
                },
                new BarberService
                {
                    Id = 2,
                    Name = "Shave",
                    Description = "Traditional shave service",
                    Price = 15,
                    IsActive = true
                });

            await context.SaveChangesAsync();

            barberServiceService = new BarberServiceService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBarberServices()
        {
            IEnumerable<BarberService> services = await barberServiceService.GetAllAsync();
            List<BarberService> result = services.ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Haircut"));
            Assert.That(result[1].Name, Is.EqualTo("Shave"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnBarberServicesOrderedById()
        {
            IEnumerable<BarberService> services = await barberServiceService.GetAllAsync();
            List<BarberService> result = services.ToList();

            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
        }
    }
}