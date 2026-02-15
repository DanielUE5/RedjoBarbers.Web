using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.ViewModels
{
    public class ReviewCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Избери услуга.")]
        public int BarberServiceId { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(100, MinimumLength = 2)]
        public string CustomerName { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Рейтингът трябва да е между 1 и 5 звезди.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Коментарът не може да надвишава 1000 символа.")]
        public string? Comments { get; set; }
    }
}