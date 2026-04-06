namespace RedjoBarbers.Web.Data.Common
{
    public static class EntityValidation
    {
        public class BarberService
        {
            public const int BarberServiceNameMaxLength = 100;
            public const int BarberServiceDescriptionMaxLength = 1000;
            public const string PriceColumnType = "decimal(6,2)";
        }

        public class Appointment
        {
            public const int CustomerNameMaxLength = 100;
            public const int CustomerEmailMaxLength = 254;
            public const int CustomerPhoneMaxLength = 20;
            public const string PhoneRegexPattern = @"^(\+359|0)8[7-9]\d\s?\d{3}\s?\d{3}$";
            public const int NotesMaxLength = 500;
        }

        public class Review
        {
            public const int CustomerNameMaxLength = 100;
            public const int CommentsMaxLength = 1000;
        }

        public class Barber
        {
            public const int BarberNameMaxLength = 100;
            public const int BarberBioMaxLength = 2000;
            public const int UrlMaxLength = 2048;
            public const int BarberPhoneNumberMaxLength = 20;
            public const string PhoneRegexPattern = @"^(\+359|0)8[7-9]\d\s?\d{3}\s?\d{3}$";
        }
    }
}
