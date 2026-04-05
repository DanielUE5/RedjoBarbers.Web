using NUnit.Framework;
using RedjoBarbers.Web.Infrastructure.Helpers;

namespace RedjoBarbers.Web.Services.Tests.HelperTests
{
    [TestFixture]
    public class UrlSecurityHelperTests
    {
        /// <summary>
        /// Verifies that the SafeHttpUrlOrNull method returns null when the input URL is null.
        /// </summary>
        /// <remarks>This test ensures that passing a null value to SafeHttpUrlOrNull does not throw an
        /// exception and correctly returns null, as expected for safe URL handling.</remarks>

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnNull_WhenUrlIsNull()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnNull_WhenUrlIsEmpty()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnNull_WhenUrlIsWhitespace()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("   ");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnNull_WhenUrlIsInvalid()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("not-a-url");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnNull_WhenSchemeIsNotHttpOrHttps()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("ftp://example.com");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnUrl_WhenHttpIsValid()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("http://example.com");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.StartWith("http://example.com"));
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldReturnUrl_WhenHttpsIsValid()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("https://example.com");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.StartWith("https://example.com"));
        }

        [Test]
        public void SafeHttpUrlOrNull_ShouldTrimUrl()
        {
            string? result = UrlSecurityHelper.SafeHttpUrlOrNull("   https://example.com   ");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.StartWith("https://example.com"));
        }

        /// <summary>
        /// Verifies that the SafeTelHrefOrNull method returns null when the phone parameter is null.
        /// </summary>
        /// <remarks>This test ensures that passing a null value to SafeTelHrefOrNull does not result in
        /// an exception and correctly returns null, as expected for invalid input.</remarks>

        [Test]
        public void SafeTelHrefOrNull_ShouldReturnNull_WhenPhoneIsNull()
        {
            string? result = UrlSecurityHelper.SafeTelHrefOrNull(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldReturnNull_WhenPhoneIsEmpty()
        {
            string? result = UrlSecurityHelper.SafeTelHrefOrNull("");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldReturnNull_WhenPhoneIsWhitespace()
        {
            string? result = UrlSecurityHelper.SafeTelHrefOrNull("   ");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldReturnNull_WhenNoValidCharacters()
        {
            string? result = UrlSecurityHelper.SafeTelHrefOrNull("abc!!!");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldReturnTelHref_WhenPhoneIsValid()
        {
            string phone = "+359 888 123 456";

            string? result = UrlSecurityHelper.SafeTelHrefOrNull(phone);

            Assert.That(result, Is.EqualTo("tel:+359 888 123 456"));
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldRemoveInvalidCharacters()
        {
            string phone = "+359-888-123-456###";

            string? result = UrlSecurityHelper.SafeTelHrefOrNull(phone);

            Assert.That(result, Is.EqualTo("tel:+359-888-123-456"));
        }

        [Test]
        public void SafeTelHrefOrNull_ShouldAllowParentheses()
        {
            string phone = "+359 (888) 123-456";

            string? result = UrlSecurityHelper.SafeTelHrefOrNull(phone);

            Assert.That(result, Is.EqualTo("tel:+359 (888) 123-456"));
        }
    }
}