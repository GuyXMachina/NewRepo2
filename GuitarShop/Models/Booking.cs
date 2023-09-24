using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuitarShop.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        [ForeignKey("Facility")]
        public int FacilityID { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserID { get; set; }  // Adjusted to string to match IdentityUser ID type

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // Consider using an enum

        // Navigation properties
        public Facility Facility { get; set; }

        public string FacilityManagerId { get; set; }
        [ForeignKey("FacilityManagerId")]
        public virtual IdentityUser FacilityManager { get; set; }

        public string FacilityInChargeId { get; set; }
        [ForeignKey("FacilityInChargeId")]
        public virtual IdentityUser FacilityInCharge { get; set; }

        public virtual IdentityUser User { get; set; }  // Made it virtual for consistency
    }

}
