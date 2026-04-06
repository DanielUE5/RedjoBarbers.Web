using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Tests.ServiceTests
{
    [TestFixture]
    public class HomeServiceTests
    {
        private RedjoBarbersDbContext context = null!;
        private HomeService homeService = null!;

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
            Guid user3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            Guid user4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            Guid user5Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            Guid user6Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

            context.BarberServices.AddRange(
                new BarberService
                {
                    Id = 1,
                    Name = "Haircut",
                    Description = "Basic haircut",
                    Price = 20,
                    IsActive = true
                },
                new BarberService
                {
                    Id = 2,
                    Name = "Beard Trim",
                    Description = "Professional beard trim",
                    Price = 15,
                    IsActive = true
                });

            context.Barbers.AddRange(
                new Barber
                {
                    Id = 1,
                    Name = "Реджеп Иванов",
                    PhoneNumber = "+359888111111",
                    Bio = "Owner barber",
                    PhotoUrl = "owner.jpg"
                },
                new Barber
                {
                    Id = 2,
                    Name = "Ivan Petrov",
                    PhoneNumber = "+359888222222",
                    Bio = "Second barber",
                    PhotoUrl = "ivan.jpg"
                });

            context.Reviews.AddRange(
                new Review
                {
                    Id = 1,
                    BarberServiceId = 1,
                    CustomerName = "Daniel",
                    Comments = "Excellent",
                    Rating = 5,
                    ReviewDate = new DateTime(2026, 4, 1, 10, 0, 0),
                    UserId = user1Id
                },
                new Review
                {
                    Id = 2,
                    BarberServiceId = 1,
                    CustomerName = "Peter",
                    Comments = "Perfect",
                    Rating = 5,
                    ReviewDate = new DateTime(2026, 4, 2, 10, 0, 0),
                    UserId = user2Id
                },
                new Review
                {
                    Id = 3,
                    BarberServiceId = 2,
                    CustomerName = "Maria",
                    Comments = "Amazing",
                    Rating = 5,
                    ReviewDate = new DateTime(2026, 4, 3, 10, 0, 0),
                    UserId = user3Id
                },
                new Review
                {
                    Id = 4,
                    BarberServiceId = 2,
                    CustomerName = "George",
                    Comments = "Great service",
                    Rating = 5,
                    ReviewDate = new DateTime(2026, 4, 4, 10, 0, 0),
                    UserId = user4Id
                },
                new Review
                {
                    Id = 5,
                    BarberServiceId = 1,
                    CustomerName = "Niki",
                    Comments = "Should not appear",
                    Rating = 4,
                    ReviewDate = new DateTime(2026, 4, 5, 10, 0, 0),
                    UserId = user5Id
                },
                new Review
                {
                    Id = 6,
                    BarberServiceId = 1,
                    CustomerName = "Alex",
                    Comments = "",
                    Rating = 5,
                    ReviewDate = new DateTime(2026, 4, 6, 10, 0, 0),
                    UserId = user6Id
                });

            await context.SaveChangesAsync();

            homeService = new HomeService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task GetHomePageDataAsync_ShouldReturnAllServices()
        {
            HomeIndexViewModel result = await homeService.GetHomePageDataAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Services, Is.Not.Null);
            Assert.That(result.Services.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetHomePageDataAsync_ShouldReturnOnlyTopFourValidReviews()
        {
            HomeIndexViewModel result = await homeService.GetHomePageDataAsync();

            Assert.That(result.Reviews, Is.Not.Null);
            Assert.That(result.Reviews.Count, Is.EqualTo(4));
            Assert.That(result.Reviews.All(r => r.Rating == 5), Is.True);
            Assert.That(result.Reviews.All(r => !string.IsNullOrWhiteSpace(r.Comments)), Is.True);
        }

        [Test]
        public async Task GetHomePageDataAsync_ShouldReturnReviewsOrderedByDate()
        {
            HomeIndexViewModel result = await homeService.GetHomePageDataAsync();
            List<Review> reviews = result.Reviews.ToList();

            Assert.That(reviews[0].CustomerName, Is.EqualTo("George"));
            Assert.That(reviews[1].CustomerName, Is.EqualTo("Maria"));
            Assert.That(reviews[2].CustomerName, Is.EqualTo("Peter"));
            Assert.That(reviews[3].CustomerName, Is.EqualTo("Daniel"));
        }

        [Test]
        public async Task GetOwnerAsync_ShouldReturnOwner()
        {
            Barber? result = await homeService.GetOwnerAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Does.Contain("Реджеп"));
            Assert.That(result.PhoneNumber, Is.EqualTo("+359888111111"));
        }

        [Test]
        public async Task GetContactsAsync_ShouldReturnAllBarbersOrderedById()
        {
            IEnumerable<Barber> contacts = await homeService.GetContactsAsync();
            List<Barber> result = contacts.ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
        }
    }
}