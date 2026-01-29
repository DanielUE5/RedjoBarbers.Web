using System.ComponentModel.DataAnnotations;
using static RedjoBarbers.Web.Common.EntityValidation.Review;

namespace RedjoBarbers.Web.Data.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CustomerNameMaxLength)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [MaxLength(CommentsMaxLength)]
        public string? Comments { get; set; }

        public DateTime ReviewDate { get; set; }

        // Foreign Key to BarberService
        public int BarberServiceId { get; set; }
        public BarberService BarberService { get; set; } = null!;
    }
}
