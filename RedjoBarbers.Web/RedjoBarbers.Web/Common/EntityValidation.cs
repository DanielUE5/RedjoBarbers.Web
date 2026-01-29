namespace RedjoBarbers.Web.Common
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
            public const string PhoneRegexPattern = @"^(?:0\d{9}|\+359\d{9}|\+\d{10,})$";
            public const int NotesMaxLength = 500;
        }

        public class Review
        {
            public const int CustomerNameMaxLength = 100;
            public const int CommentsMaxLength = 1000;
        }
    }
}
