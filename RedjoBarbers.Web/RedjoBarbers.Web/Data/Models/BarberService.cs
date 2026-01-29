using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static RedjoBarbers.Web.Common.EntityValidation.BarberService;

namespace RedjoBarbers.Web.Data.Models
{
    public class BarberService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(BarberServiceNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(BarberServiceDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Column(TypeName = PriceColumnType)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
