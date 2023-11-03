using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UFSFacilityManagement.Models
{
    public class FacilityManager : User
    {
        public List<Facility> Facilities { get; set; }

        // Add any more fields specific to a Facility Manager
    }
}
