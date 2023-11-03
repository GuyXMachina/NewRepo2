using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UFSFacilityManagement.Models
{
    public class FacilityInCharge : User
    {

        // Booking details
        public ICollection<Booking> AssignedBookings { get; set; }
    }
}
