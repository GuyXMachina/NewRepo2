using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models
{
    public class FacilityInCharge : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }

        // Booking details
        public ICollection<Booking> AssignedBookings { get; set; }
    }
}
