using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UFSFacilityManagement.Models;
using UFSFacilityManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using FireSharp;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace UFSFacilityManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, FirebaseClient firebaseClient)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _firebaseClient = firebaseClient;
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
                var userFire = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(loginModel.Email);
                if (user != null && userFire != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user,
                      loginModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "You have successfully logged in.";
                        // Redirect based on role
                        // Fetch user data from Firebase
                        var firebaseResponse = await _firebaseClient.GetAsync($"Users/{user.UserName}");
                        
                        var firebaseUserData = firebaseResponse.ResultAs<User>();
                        
                        if (firebaseUserData != null)
                        {
                            Console.WriteLine("User data fetched from Firebase successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to fetch user data from Firebase.");
                        }
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("FacilityAdmin"))
                        {
                            return Redirect("/Facilities");
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
                    Email = registerModel.Email,
                    UserType = registerModel.UserType,
                    StudentNumber = registerModel.StudentNumber,
                    PassportNumber = registerModel.PassportNumber,
                    EmployeeID = registerModel.EmployeeID, 
                    ProfilePictureUrl = registerModel.ProfilePictureUrl
                };
                var userArgs = new UserRecordArgs()
                {
                    Uid = Guid.NewGuid().ToString(), // Generate a new unique user ID
                    Email = registerModel.Email,
                    DisplayName = registerModel.UserName,
                    PhotoUrl = registerModel.ProfilePictureUrl,
                    Password = registerModel.Password, // You'd need to include this if you're setting a password
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    Console.WriteLine("User creation succeeded.");
                    TempData["success"] = "You have successfully created an Account.";  // Added success TempData
                    var firebaseResponse = await _firebaseClient.SetAsync($"Users/{userArgs.DisplayName}", userArgs);
                    var fire = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
                    if (firebaseResponse.StatusCode == System.Net.HttpStatusCode.OK && fire != null)
                    {
                        Console.WriteLine("User data stored in Firebase successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to store user data in Firebase.");
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Profile", "User");  // Redirect to Home/Index or any other page
                }
                else
                {
                    Console.WriteLine("User creation failed.");
                    ModelState.AddModelError("", "User Already exists Try signing in");
                    TempData["warning"] = "Failed to create an Account.";  
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
