using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models
{
    public class FacilityManager : User
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public List<Facility> Facilities { get; set; }

        // Add any more fields specific to a Facility Manager
    }
}
