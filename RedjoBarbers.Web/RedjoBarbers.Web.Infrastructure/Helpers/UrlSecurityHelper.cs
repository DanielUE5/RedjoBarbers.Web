namespace RedjoBarbers.Web.Infrastructure.Helpers
{
    public static class UrlSecurityHelper
    {
        public static string? SafeHttpUrlOrNull(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            url = url.Trim();

            bool isValidUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);

            if (!isValidUri || uri == null)
            {
                return null;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                return null;
            }

            return uri.ToString();
        }

        public static string? SafeTelHrefOrNull(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            string normalized = new string(phoneNumber
                .Where(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-' || c == '(' || c == ')')
                .ToArray());

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return null;
            }

            return $"tel:{normalized}";
        }
    }
}