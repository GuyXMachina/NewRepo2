using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UFSFacilityManagement.Models.ViewModels
{

    public class LoginModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; } = "/";
    }

    public class RegisterModel : User
    {
        [Required(ErrorMessage = "User type is required.")]
        [Display(Name = "User Type")]
        public UserType UserType { get; set; }

        [Required(ErrorMessage = "Name and surname are required.")]
        [Display(Name = "Name and Surname")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Student Number")]
        public string StudentNumber { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "Passport Or ID Number")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords must match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


}

