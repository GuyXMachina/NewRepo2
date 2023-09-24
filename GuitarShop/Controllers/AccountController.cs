using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GuitarShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
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
                IdentityUser user =
                  await _userManager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user,
                      loginModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        // Redirect based on role
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("FacilityAdmin"))
                        {
                            return Redirect("/Admin/Dashboard");
                        }
                        else if (roles.Contains("FacilityManager"))
                        {
                            return Redirect("/Manager/Dashboard");
                        }
                        else if (roles.Contains("FacilityInCharge"))
                        {
                            return Redirect("/InCharge/Dashboard");
                        }
                        else if (roles.Contains("User"))
                        {
                            return Redirect("/User/Dashboard");
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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                // Create roles if not exist
                var roles = new List<string> { "FacilityAdmin", "FacilityManager", "FacilityInCharge", "User" };
                foreach (var role in roles)
                {
                    if (await _roleManager.FindByNameAsync(role) == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var user = new IdentityUser
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    // Assign role logic here. For now, adding to "User" role.
                    await _userManager.AddToRoleAsync(user, "User");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to register new user");
                }
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
