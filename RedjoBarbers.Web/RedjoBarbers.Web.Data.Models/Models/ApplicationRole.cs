using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.Data.Models.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string? Label { get; set; }
    }
}
