using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models
{
    public class FacilityInCharge : User
    {

        // Booking details
        public ICollection<Booking> AssignedBookings { get; set; }
    }
}
