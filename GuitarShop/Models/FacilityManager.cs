using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models
{
    public class FacilityManager
    {
        [Key]
        public int FacilityManagerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public User IdentityUser { get; set; }
        public List<Facility> Facilities { get; set; }

        // Add any more fields specific to a Facility Manager
    }
}
