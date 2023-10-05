using Microsoft.AspNetCore.Identity;

namespace GuitarShop.Models
{
    public enum UserType
    {
        Student,
        Visitor,
        Staff
    }
    public class User : IdentityUser
    {
        public UserType UserType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string StudentNumber { get; set; }
        public string EmployeeID { get; set; }
        public string PassportNumber { get; set; }
       
    }
}
