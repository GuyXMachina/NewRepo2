using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly AppDbContext _context;

        public FacilityAdminController(IRepositoryWrapper repoWrapper, UserManager<User> userManager, AppDbContext appDbContext)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
            _context = appDbContext;
        }

        public IActionResult Facilities()
        {
            var facilities = _repoWrapper.Facility.FindAll();
            return View(facilities);
        }

        [HttpGet]
        public IActionResult CreateFacility()
        {
            var facility = new Facility();
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
            return View(facility);
        }

        [HttpPost]
        public IActionResult CreateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                _context.Facilities.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Facilities");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateFacility(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
            return View(facility);
        }

        [HttpPost]
        public IActionResult UpdateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine($"Updating facility with ID: {model.FacilityID}, CategoryID: {model.CategoryID}");
                    if (model.CategoryID <= 0)
                    {
                        Console.WriteLine("Invalid CategoryID");
                        ModelState.AddModelError("CategoryID", "Invalid CategoryID");
                        return View(model);
                    }

                    _context.Facilities.Update(model);
                    _context.SaveChanges();
                    return RedirectToAction("Facilities");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while updating the facility: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Model state is not valid.");
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
            Console.WriteLine($"Received ID: {id}");  // Debugging line
            var facility = _context.Facilities.Find(id);
            if (facility == null)
            {
                TempData["ErrorMessage"] = "Facility not found";
                return RedirectToAction("Facilities");
            }
            _context.Facilities.Remove(facility);
            _context.SaveChanges();
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
        public async Task<IActionResult> UpdateFacilityManager(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(model.Id);
      
                if (existingUser != null)
                {
                    // Update only the fields you want to change
                    existingUser.Name = model.Name;
                    existingUser.Surname = model.Surname;
                    existingUser.Password = model.Password;
                    existingUser.Email = model.Email;
                    // ... any other fields you want to update

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("FacilityManagers");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    return NotFound();
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
                return NotFound("Facility Manager not found.");
            }
            return View(manager);
        }

        [HttpPost, ActionName("DeleteFacilityManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFacilityManagerConfirmed(string id)
        {
            var manager = await _userManager.FindByIdAsync(id);
            if (manager == null)
            {
                return NotFound("Facility Manager not found.");
            }
            var result = await _userManager.DeleteAsync(manager);
            if (result.Succeeded)
            {
                TempData["Message"] = "Facility Manager has been deleted.";
                return RedirectToAction("FacilityManagers");
            }
            else
            {
                // If the operation failed, you can log the errors here
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("DeleteFacilityManager", manager);  // Show the view again with the errors
            }
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
