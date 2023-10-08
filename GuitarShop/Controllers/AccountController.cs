using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Models;
using GuitarShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace GuitarShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user =
                  await _userManager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user,
                      loginModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "You have successfully logged in.";
                        // Redirect based on role
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("FacilityAdmin"))
                        {
                            return Redirect("/FacilityAdmin/Facilities");
                        }
                        else if (roles.Contains("FacilityManager"))
                        {
                            return Redirect("/FacilityManager/Index");
                        }
                        else if (roles.Contains("FacilityInCharge"))
                        {
                            return Redirect("/FacilityInCharge/BoookingDetails");
                        }
                        else if (roles.Contains("User"))
                        {
                            return Redirect("/User/Profile");
                        }
                        // ... Add more roles as needed
                        
                        return Redirect(loginModel?.ReturnUrl ?? "/Home/Index");
                    }

                }
            }
            ModelState.AddModelError("", "Invalid email or password");
            return View(loginModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            Console.WriteLine("Entering GET Register method");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            Console.WriteLine("Entering POST Register method");

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid.");

                var roles = new List<string> { "FacilityAdmin", "FacilityManager", "FacilityInCharge", "User" };
                foreach (var role in roles)
                {
                    if (await _roleManager.FindByNameAsync(role) == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var user = new User
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    Console.WriteLine("User creation succeeded.");
                    TempData["success"] = "You have successfully created an Account.";  // Added success TempData
                    await _userManager.AddToRoleAsync(user, "User");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Console.WriteLine("User creation failed.");
                    ModelState.AddModelError("", "Unable to register new user");
                    TempData["warning"] = "Failed to create an Account.";  // Added warning TempData
                }
            }
            else
            {
                Console.WriteLine("Model state is invalid.");
                TempData["warning"] = "Invalid data entered.";  // Added warning TempData
            }

            return View(registerModel);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
