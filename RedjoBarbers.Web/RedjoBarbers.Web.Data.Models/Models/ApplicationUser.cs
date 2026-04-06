using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.Data.Models.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
