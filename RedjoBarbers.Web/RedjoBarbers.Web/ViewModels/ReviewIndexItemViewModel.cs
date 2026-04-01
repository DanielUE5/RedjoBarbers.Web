using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.ViewModels
{
    public class ReviewIndexItemViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Невалидна услуга.")]
        public int BarberServiceId { get; set; }

        [Required(ErrorMessage = "Името на услугата е задължително.")]
        [StringLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Името на клиента е задължително.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Името трябва да е между 2 и 100 символа.")]
        public string CustomerName { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Рейтингът трябва да бъде между 1 и 5 звезди.")]
        public int Rating { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

        [StringLength(1000, ErrorMessage = "Коментарът не може да надвишава 1000 символа.")]
        public string? Comments { get; set; }
    }
}
