using GuitarShop.Data;
using GuitarShop.Models.ViewModels;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GuitarShop.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["warning"] = "User not found.";
                return RedirectToAction("Login");
            }

            User model = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                UserType = user.UserType,
                StudentNumber = user.StudentNumber,
                EmployeeID = user.EmployeeID,
                PassportNumber = user.PassportNumber
            };
            TempData["success"] = "Logged In Successfully";

            return View(model);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["warning"] = "Please correct the form errors.";
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["warning"] = "User not found. Please log in again.";
                return RedirectToAction("Login");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                TempData["info"] = "Password change failed. See below for details.";
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            TempData["success"] = "Password Changed Successfully";
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }

    }
}
