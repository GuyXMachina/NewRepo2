using UFSFacilityManagement.Data;
using UFSFacilityManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net.Mail;
using System.Net;

namespace UFSFacilityManagement.Controllers
{
    [Authorize(Roles = "FacilityAdmin")]
    public class FacilityAdminController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
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
        public IActionResult ListUsers()
        {
            return View(_context.Users.ToList<User>());
        }
        [HttpPost]
        public async Task<IActionResult> ProcessUsers()
        {
            // Fetch all transactions from the database
            var people = await _context.UserS.ToListAsync();

            if (people != null && people.Count > 0)
            {
                GeneratePdf(people);

                await SendEmailWithPdfAsync();
            }

            return RedirectToAction("ListUsers");
        }

        public void GeneratePdf(List<User> people)
        {
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream("UserReport.pdf", FileMode.Create));
            doc.Open();

            foreach (var person in people)
            {
                doc.Add(new Paragraph($"Type: {person.UserType}"));
                doc.Add(new Paragraph($"Booking: {person.UserName}"));
                doc.Add(new Paragraph($"Email: {person.Email}"));
                doc.Add(new Paragraph("------------------------------------------------"));
            }

            doc.Close();
        }
        public async Task SendEmailWithPdfAsync()
        {
            string fromMail = "thembi1018@gmail.com";
            string password = "cyis kdhf pmpl ansl";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromMail);
            mail.To.Add(new MailAddress("thembimthimkulu8@gmail.com"));
            mail.Subject = "User Report";
            mail.Body = "Attached is the user report.";

            Attachment attachment;
            attachment = new Attachment("UserReport.pdf");
            mail.Attachments.Add(attachment);
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, password),
                EnableSsl = true
            };


            await SmtpServer.SendMailAsync(mail);
        }

        //[HttpGet]
        //public IActionResult CreateFacility()
        //{
        //    var facility = new Facility();
        //    ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
        //    return View(facility);
        //}

        //[HttpPost]
        //public IActionResult CreateFacility(Facility model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Facilities.Add(model);
        //        _context.SaveChanges();
        //        return RedirectToAction("Facilities");
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public IActionResult UpdateFacility(int id)
        //{
        //    var facility = _repoWrapper.Facility.GetById(id);
        //    ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
        //    return View(facility);
        //}

        //[HttpPost]
        //public IActionResult UpdateFacility(Facility model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            Console.WriteLine($"Updating facility with ID: {model.FacilityID}, CategoryID: {model.CategoryID}");
        //            if (model.CategoryID <= 0)
        //            {
        //                Console.WriteLine("Invalid CategoryID");
        //                ModelState.AddModelError("CategoryID", "Invalid CategoryID");
        //                return View(model);
        //            }

        //            _context.Facilities.Update(model);
        //            _context.SaveChanges();
        //            return RedirectToAction("Facilities");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"An error occurred while updating the facility: {ex.Message}");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Model state is not valid.");
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public IActionResult DeleteFacility(int id)
        //{
        //    var facility = _repoWrapper.Facility.GetById(id);
        //    return View(facility);
        //}

        //[HttpPost]
        //public IActionResult DeleteFacilityConfirmed(int id)
        //{
        //    Console.WriteLine($"Received ID: {id}");  // Debugging line
        //    var facility = _context.Facilities.Find(id);
        //    if (facility == null)
        //    {
        //        TempData["ErrorMessage"] = "Facility not found";
        //        return RedirectToAction("Facilities");
        //    }
        //    _context.Facilities.Remove(facility);
        //    _context.SaveChanges();
        //    return RedirectToAction("Facilities");
        //}




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
                    UserName = model.Name + " "+ model.Surname,
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

            // Find all bookings related to this Facility Manager
            var relatedBookings = _context.Bookings.Where(b => b.FacilityManagerId == id);

            // Unlink or reassign the Facility Manager for these bookings
            foreach (var booking in relatedBookings)
            {
                booking.FacilityManagerId = null; // or assign to another Facility Manager
            }
            await _context.SaveChangesAsync();

            // Now delete the Facility Manager
            var result = await _userManager.DeleteAsync(manager);
            if (result.Succeeded)
            {
                TempData["success"] = "Facility Manager has been deleted.";
                return RedirectToAction("FacilityManagers");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("DeleteFacilityManager", manager);
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
