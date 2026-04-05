using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RedjoBarbers.Web.Controllers;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;

namespace RedjoBarbers.Web.IntegrationTests.Tests.ControllerTests
{
    [TestFixture]
    public class BarberServicesControllerTests
    {
        private Mock<IBarberServiceService> barberServiceServiceMock = null!;
        private BarberServicesController controller = null!;

        [SetUp]
        public void Setup()
        {
            barberServiceServiceMock = new Mock<IBarberServiceService>();
            controller = new BarberServicesController(barberServiceServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithAllServices()
        {
            IEnumerable<BarberService> services = new List<BarberService>
            {
                new BarberService(),
                new BarberService()
            };

            barberServiceServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(services);

            IActionResult result = await controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(services));
        }
    }
}