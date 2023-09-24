using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GuitarShop.Controllers
{
    [Authorize(Roles = "FacilityInCharge")]
    public class FacilityInChargeController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public FacilityInChargeController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        // Profile Page
        public IActionResult Profile()
        {
            var facilityInCharge = _repoWrapper.FacilityInCharge.GetProfile(User.Identity.Name); 
            return View(facilityInCharge);
        }

        // View Booking Details
        public IActionResult BookingDetails()
        {
            var bookings = _repoWrapper.Booking.GetBookingsByStatus(User.Identity.Name); 
            return View(bookings);
        }

        // Update Booking Status
        [HttpPost]
        public IActionResult UpdateStatus(int bookingId, string newStatus)
        {
            _repoWrapper.Booking.ChangeBookingStatus(bookingId, newStatus); 
            _repoWrapper.Save();
            return RedirectToAction("BookingDetails");
        }
    }
}
