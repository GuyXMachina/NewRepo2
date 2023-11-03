using UFSFacilityManagement.Models.ViewModels;
using UFSFacilityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UFSFacilityManagement.Controllers
{
    public class FacilityManagerController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public FacilityManagerController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
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
                TempData["warning"] = "User not found.";
                return RedirectToAction("Login");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(TempData["info"] = "Password change failed. See below for details.");
            }

            
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index");
        }

    }
}
