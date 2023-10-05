using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GuitarShop.Controllers
{
    [Authorize(Roles = "FacilityAdmin")]
    public class FacilityAdminController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public FacilityAdminController(IRepositoryWrapper repoWrapper, UserManager<User> userManager)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
        }

        public IActionResult Facilities()
        {
            var facilities = _repoWrapper.Facility.FindAll();
            return View(facilities);
        }

        [HttpGet]
        public IActionResult CreateFacility()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                _repoWrapper.Facility.Create(model);
                _repoWrapper.Save();
                return RedirectToAction("Facilities");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateFacility(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            return View(facility);
        }

        [HttpPost]
        public IActionResult UpdateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                _repoWrapper.Facility.Update(model);
                _repoWrapper.Save();
                return RedirectToAction("Facilities");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteFacility(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            return View(facility);
        }

        [HttpPost]
        public IActionResult DeleteFacilityConfirmed(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            if (facility != null)
            {
                _repoWrapper.Facility.Delete(facility);
                _repoWrapper.Save();
            }
            return RedirectToAction("Facilities");
        }
        [HttpGet]
        public async Task<IActionResult> FacilityManagers()
        {
            var managers = await _userManager.GetUsersInRoleAsync("FacilityManager");

            // Debug prints
            Console.WriteLine($"Fetched {managers.Count()} managers.");
            foreach (var manager in managers)
            {
                Console.WriteLine($"Manager ID: {manager.EmployeeID}, Name: {manager.Name}");
            }

            return View(managers);
        }

        [HttpGet]
        public ViewResult CreateFacilityManager()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFacilityManager(User model)
        {
            Console.WriteLine("Entered method");  // Debugging line
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "This email is already used!");
                    return View(model);
                }
                Console.WriteLine("Model is valid");  // Debugging line
                User user = new()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Name + model.Surname,
                    Email = model.Email,
                    UserType = UserType.Staff
                };
                // Create FacilityManager asynchronously
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Console.WriteLine("User created successfully");  // Debugging line
                    // Add FacilityManager role to user
                    await _userManager.AddToRoleAsync(user, "FacilityManager");
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("FacilityManagers");
                }
                else
                {
                    Console.WriteLine("User creation failed");  // Debugging line
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                Console.WriteLine("Model is invalid");  // Debugging line
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> UpdateFacilityManager(string id)
        {
            var manager = await _userManager.FindByIdAsync(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFacilityManager(FacilityManager model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.UpdateAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("FacilityManagers");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFacilityManager(string id)
        {
            var manager = await _userManager.FindByIdAsync(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFacilityManagerConfirmed(FacilityManager manager)
        {
            if (manager != null)
            {
                var result = await _userManager.DeleteAsync(manager);
                if (result.Succeeded)
                {
                    TempData["Message"] = $"{manager.Name} {manager.Surname} has been deleted";
                    return RedirectToAction("FacilityManagers");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View("FacilityManagers");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }


    }

}
