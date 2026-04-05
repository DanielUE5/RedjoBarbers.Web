using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Tests.ServiceTests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private RedjoBarbersDbContext context;
        private ReviewService reviewService;

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

            context.Reviews.AddRange(
                new Review
                {
                    Id = 1,
                    BarberServiceId = 1,
                    CustomerName = "Daniel",
                    Rating = 5,
                    Comments = "Excellent",
                    ReviewDate = new DateTime(2026, 4, 1, 10, 0, 0),
                    UserId = "user1"
                },
                new Review
                {
                    Id = 2,
                    BarberServiceId = 1,
                    CustomerName = "Peter",
                    Rating = 4,
                    Comments = "Very good",
                    ReviewDate = new DateTime(2026, 4, 2, 10, 0, 0),
                    UserId = "user2"
                },
                new Review
                {
                    Id = 3,
                    BarberServiceId = 2,
                    CustomerName = "Maria",
                    Rating = 3,
                    Comments = "Average",
                    ReviewDate = new DateTime(2026, 4, 3, 10, 0, 0),
                    UserId = "user3"
                },
                new Review
                {
                    Id = 4,
                    BarberServiceId = 2,
                    CustomerName = "George",
                    Rating = 5,
                    Comments = "Amazing service",
                    ReviewDate = new DateTime(2026, 4, 4, 10, 0, 0),
                    UserId = "user4"
                },
                new Review
                {
                    Id = 5,
                    BarberServiceId = 1,
                    CustomerName = "Niki",
                    Rating = 2,
                    Comments = "Not great",
                    ReviewDate = new DateTime(2026, 4, 5, 10, 0, 0),
                    UserId = "user5"
                },
                new Review
                {
                    Id = 6,
                    BarberServiceId = 2,
                    CustomerName = "Alex",
                    Rating = 1,
                    Comments = "Bad experience",
                    ReviewDate = new DateTime(2026, 4, 6, 10, 0, 0),
                    UserId = "user6"
                });

            await context.SaveChangesAsync();

            reviewService = new ReviewService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllReviews_WhenNoFilterIsApplied()
        {
            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(null, null, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Reviews, Is.Not.Null);
            Assert.That(result.TotalCount, Is.EqualTo(6));
            Assert.That(result.Reviews.Count, Is.EqualTo(6));
            Assert.That(result.Page, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(6));
        }

        [Test]
        public async Task GetAllAsync_ShouldFilterByBarberServiceId()
        {
            int barberServiceId = 2;

            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(barberServiceId, null, 1);

            Assert.That(result.TotalCount, Is.EqualTo(3));
            Assert.That(result.Reviews.Count, Is.EqualTo(3));
            Assert.That(result.Reviews.All(r => r.BarberServiceId == 2), Is.True);
        }

        [Test]
        public async Task GetAllAsync_ShouldSortByNewest_WhenSortReviewsIsNewest()
        {
            string sortReviews = "newest";

            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(null, sortReviews, 1);
            List<ReviewIndexItemViewModel> reviews = result.Reviews.ToList();

            Assert.That(reviews[0].CustomerName, Is.EqualTo("Alex"));
            Assert.That(reviews[1].CustomerName, Is.EqualTo("Niki"));
            Assert.That(reviews[2].CustomerName, Is.EqualTo("George"));
            Assert.That(reviews[3].CustomerName, Is.EqualTo("Maria"));
            Assert.That(reviews[4].CustomerName, Is.EqualTo("Peter"));
            Assert.That(reviews[5].CustomerName, Is.EqualTo("Daniel"));
        }

        [Test]
        public async Task GetAllAsync_ShouldSortByRatingDescendingThenByReviewDateDescending_WhenSortReviewsIsNull()
        {
            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(null, null, 1);
            List<ReviewIndexItemViewModel> reviews = result.Reviews.ToList();

            Assert.That(reviews[0].Rating, Is.EqualTo(5));
            Assert.That(reviews[1].Rating, Is.EqualTo(5));
            Assert.That(reviews[2].Rating, Is.EqualTo(4));
            Assert.That(reviews[3].Rating, Is.EqualTo(3));
            Assert.That(reviews[4].Rating, Is.EqualTo(2));
            Assert.That(reviews[5].Rating, Is.EqualTo(1));

            Assert.That(reviews[0].CustomerName, Is.EqualTo("George"));
            Assert.That(reviews[1].CustomerName, Is.EqualTo("Daniel"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnPageOne_WhenCurrentPageIsLessThanOne()
        {
            int currentPage = 0;

            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(null, null, currentPage);

            Assert.That(result.Page, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnServicesForDropdown()
        {
            ReviewIndexPageViewModel result = await reviewService.GetAllAsync(null, null, 1);
            List<SelectListItem> services = result.Services.ToList();

            Assert.That(services.Count, Is.EqualTo(2));
            Assert.That(services[0].Text, Is.EqualTo("Beard Trim"));
            Assert.That(services[1].Text, Is.EqualTo("Haircut"));
        }

        [Test]
        public async Task GetAllServicesAsync_ShouldReturnAllServicesOrderedByName()
        {
            IEnumerable<BarberService> services = await reviewService.GetAllServicesAsync();
            List<BarberService> result = services.ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Beard Trim"));
            Assert.That(result[1].Name, Is.EqualTo("Haircut"));
        }

        [Test]
        public async Task GetCreateModelAsync_ShouldReturnModelWithPassedBarberServiceId()
        {
            int barberServiceId = 2;

            ReviewCreateViewModel result = await reviewService.GetCreateModelAsync(barberServiceId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BarberServiceId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetCreateModelAsync_ShouldReturnModelWithZero_WhenBarberServiceIdIsNull()
        {
            ReviewCreateViewModel result = await reviewService.GetCreateModelAsync(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BarberServiceId, Is.EqualTo(0));
        }

        [Test]
        public async Task CreateAsync_ShouldAddReviewToDatabase()
        {
            ReviewCreateViewModel model = new ReviewCreateViewModel
            {
                BarberServiceId = 1,
                CustomerName = "New User",
                Rating = 5,
                Comments = "Fantastic"
            };

            bool result = await reviewService.CreateAsync(model, "new-user");

            Assert.That(result, Is.True);
            Assert.That(context.Reviews.Count(), Is.EqualTo(7));

            Review? createdReview = context.Reviews
                .FirstOrDefault(r => r.UserId == "new-user" && r.CustomerName == "New User");

            Assert.That(createdReview, Is.Not.Null);
            Assert.That(createdReview!.Comments, Is.EqualTo("Fantastic"));
            Assert.That(createdReview.Rating, Is.EqualTo(5));
            Assert.That(createdReview.BarberServiceId, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateAsync_ShouldSaveEmptyString_WhenCommentsIsNull()
        {
            ReviewCreateViewModel model = new ReviewCreateViewModel
            {
                BarberServiceId = 2,
                CustomerName = "Null Comment User",
                Rating = 4,
                Comments = null
            };

            bool result = await reviewService.CreateAsync(model, "null-comment-user");

            Assert.That(result, Is.True);

            Review? createdReview = context.Reviews
                .FirstOrDefault(r => r.UserId == "null-comment-user");

            Assert.That(createdReview, Is.Not.Null);
            Assert.That(createdReview!.Comments, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task IsOwnerAsync_ShouldReturnTrue_WhenUserOwnsReview()
        {
            bool result = await reviewService.IsOwnerAsync(1, "user1");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsOwnerAsync_ShouldReturnFalse_WhenUserDoesNotOwnReview()
        {
            bool result = await reviewService.IsOwnerAsync(1, "different-user");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsOwnerOrAdminAsync_ShouldReturnTrue_WhenUserIsAdminAndReviewExists()
        {
            bool result = await reviewService.IsOwnerOrAdminAsync(1, "different-user", true);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsOwnerOrAdminAsync_ShouldReturnTrue_WhenUserOwnsReview()
        {
            bool result = await reviewService.IsOwnerOrAdminAsync(2, "user2", false);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsOwnerOrAdminAsync_ShouldReturnFalse_WhenUserIsNotOwnerAndNotAdmin()
        {
            bool result = await reviewService.IsOwnerOrAdminAsync(2, "other-user", false);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetUserReviewsAsync_ShouldReturnOnlyReviewsForGivenUser()
        {
            IEnumerable<Review> reviews = await reviewService.GetUserReviewsAsync("user1");
            List<Review> result = reviews.ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo("user1"));
            Assert.That(result[0].CustomerName, Is.EqualTo("Daniel"));
        }

        [Test]
        public async Task GetUpdateModelAsync_ShouldReturnModel_WhenReviewExists()
        {
            ReviewUpdateViewModel? result = await reviewService.GetUpdateModelAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.CustomerName, Is.EqualTo("Daniel"));
            Assert.That(result.Rating, Is.EqualTo(5));
            Assert.That(result.Comments, Is.EqualTo("Excellent"));
        }

        [Test]
        public async Task GetUpdateModelAsync_ShouldReturnNull_WhenReviewDoesNotExist()
        {
            ReviewUpdateViewModel? result = await reviewService.GetUpdateModelAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnFalse_WhenReviewDoesNotExist()
        {
            ReviewUpdateViewModel model = new ReviewUpdateViewModel
            {
                Id = 999,
                BarberServiceId = 1,
                CustomerName = "Missing",
                Rating = 3,
                Comments = "Missing review"
            };

            bool result = await reviewService.UpdateAsync(model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateReview_WhenReviewExists()
        {
            ReviewUpdateViewModel model = new ReviewUpdateViewModel
            {
                Id = 1,
                BarberServiceId = 1,
                CustomerName = "Daniel Updated",
                Rating = 4,
                Comments = "Updated comment"
            };

            bool result = await reviewService.UpdateAsync(model);

            Assert.That(result, Is.True);

            Review? updatedReview = await context.Reviews.FindAsync(1);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview!.CustomerName, Is.EqualTo("Daniel Updated"));
            Assert.That(updatedReview.Rating, Is.EqualTo(4));
            Assert.That(updatedReview.Comments, Is.EqualTo("Updated comment"));
        }

        [Test]
        public async Task UpdateAsync_ShouldSetEmptyString_WhenCommentsIsNull()
        {
            ReviewUpdateViewModel model = new ReviewUpdateViewModel
            {
                Id = 2,
                BarberServiceId = 1,
                CustomerName = "Peter Updated",
                Rating = 5,
                Comments = null
            };

            bool result = await reviewService.UpdateAsync(model);

            Assert.That(result, Is.True);

            Review? updatedReview = await context.Reviews.FindAsync(2);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview!.Comments, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenReviewDoesNotExist()
        {
            bool result = await reviewService.DeleteAsync(999);

            Assert.That(result, Is.False);
            Assert.That(context.Reviews.Count(), Is.EqualTo(6));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveReview_WhenReviewExists()
        {
            bool result = await reviewService.DeleteAsync(1);

            Assert.That(result, Is.True);
            Assert.That(context.Reviews.Count(), Is.EqualTo(5));
            Assert.That(await context.Reviews.FindAsync(1), Is.Null);
        }
    }
}