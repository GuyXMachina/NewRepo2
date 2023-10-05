using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IRepositoryWrapper _bookingRepository;

        public BookingController(IRepositoryWrapper bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public IActionResult Index()
        {
            var bookings = _bookingRepository.Booking.FindAll();
            return View(bookings);
        }

        public IActionResult Details(string id)
        {
            var booking = _bookingRepository.Booking.GetById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _bookingRepository.Booking.Create(booking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var booking = _bookingRepository.Booking.GetById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _bookingRepository.Booking.Update(booking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        [HttpGet]
        [Authorize(Roles = "FacilityAdmin, FacilityInCharge")]
        public IActionResult Delete(string id)
        {
            var booking = _bookingRepository.Booking.GetById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FacilityAdmin, FacilityInCharge")]
        public IActionResult DeleteConfirmed(string id)
        {
            var booking = _bookingRepository.Booking.GetById(id);
            if (booking != null)
            {
                _bookingRepository.Booking.Delete(booking);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
