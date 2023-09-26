using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models.ViewModels
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

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }
        [Display(Name = "Employee ID")]
        public string EmployeeNumber { get; set; }
        [Display(Name = "Passport Or ID Number")]
        public string PassportNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords must match")]
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

    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string StudentNumber { get; set; }
        public string PassportNumberOrIDNo { get; set; }
        public string EmployeeID { get; set; }
        public string UserType { get; set; }  // Could be "Student", "Visitor", or "Staff"
    }


}

