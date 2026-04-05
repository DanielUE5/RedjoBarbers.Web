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
    public class HomeControllerTests
    {
        private Mock<IHomeService> homeServiceMock = null!;
        private HomeController controller = null!;

        [SetUp]
        public void Setup()
        {
            homeServiceMock = new Mock<IHomeService>();
            controller = new HomeController(homeServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithHomePageData()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();

            homeServiceMock
                .Setup(x => x.GetHomePageDataAsync())
                .ReturnsAsync(model);

            IActionResult result = await controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Privacy_ShouldReturnViewWithOwner()
        {
            Barber owner = new Barber();

            homeServiceMock
                .Setup(x => x.GetOwnerAsync())
                .ReturnsAsync(owner);

            IActionResult result = await controller.Privacy();

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(owner));
        }

        [Test]
        public void About_ShouldReturnView()
        {
            IActionResult result = controller.About();

            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public async Task Contacts_ShouldReturnViewWithContacts()
        {
            IEnumerable<Barber> contacts = new List<Barber>
            {
                new Barber(),
                new Barber()
            };

            homeServiceMock
                .Setup(x => x.GetContactsAsync())
                .ReturnsAsync(contacts);

            IActionResult result = await controller.Contacts();

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(contacts));
        }

        [Test]
        public void Error_ShouldReturnError404View_WhenStatusCodeIs404()
        {
            IActionResult result = controller.Error(404);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Error404"));
        }

        [Test]
        public void Error_ShouldReturnError500View_WhenStatusCodeIs500()
        {
            IActionResult result = controller.Error(500);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Error500"));
        }

        [Test]
        public void Error_ShouldReturnDefaultErrorView_WhenStatusCodeIsNull()
        {
            IActionResult result = controller.Error(null);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));
        }

        [Test]
        public void Error_ShouldReturnDefaultErrorView_WhenStatusCodeIsDifferent()
        {
            IActionResult result = controller.Error(403);

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));
        }
    }
}