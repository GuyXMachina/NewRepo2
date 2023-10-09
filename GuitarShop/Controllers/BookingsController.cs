using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GuitarShop.Data;
using GuitarShop.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Stripe.Checkout;
using Stripe;

namespace GuitarShop.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Bookings.Include(b => b.Facility).Include(b => b.FacilityInCharge).Include(b => b.FacilityManager).Include(b => b.User);
            return View(await appDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult CalendarBook()
        {
            var bookings = _context.Bookings.Select(b => new 
            {
                title = b.Facility.Name,
                start = b.StartTime,
                end = b.EndTime,
                url = Url.Action("Details", "Bookings", new { id = b.BookingID})
            }).ToList();
            ViewData["Bookings"] = JsonConvert.SerializeObject(bookings);
            return View(bookings);
        }
        public IActionResult CheckOut(int id)
        {
            var facility = _context.Facilities.Find(id);
            if (facility == null)
            {
                return NotFound();
            }
            var booking = _context.Bookings
                          .Where(b => b.FacilityID == id)
                          .OrderByDescending(b => b.BookingID)
                          .FirstOrDefault();

            if (booking == null)
            {
                return NotFound(); // or some other appropriate action
            }

            int bookingId = booking.BookingID;

            string scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.Value;
            var domain = $"{scheme}://{host}";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"/Bookings/Success",
                CancelUrl = domain + "/Bookings/Cancel",
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {

            PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = Convert.ToInt64(facility.DiscountPrice * 100),
                    Currency = "zar",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = facility.Name,
                    },
                },
                Quantity = 1,
            }
        },
                Mode = "payment"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            TempData["BookingID"] = bookingId;
            TempData["Price"] = facility.DiscountPrice.ToString("G");

            return new StatusCodeResult(303);
        }


        public async Task<IActionResult> Success()
        {

            try
            {
                if (TempData["BookingID"] != null && TempData["Price"] != null)
                {
                    int bookingId = Convert.ToInt32(TempData["BookingID"]);
                    decimal price = Convert.ToDecimal(TempData["Price"]);
                    var booking = await _context.Bookings.FindAsync(bookingId);

                    if (booking != null)
                    {
                        booking.Transaction = new Transaction
                        {
                            TransactionDate = DateTime.Now,
                            PaymentMethod = "Credit Card",
                            Amount = price
                        };

                        booking.Status = "Paid";
                        _context.Bookings.Update(booking);
                        await _context.SaveChangesAsync();
                    }
                }
                return View();
            }
            catch (StripeException)
            {
                // Invalid payload or signature. Return 400 Bad Request
                return BadRequest();
            }
        }
        public IActionResult Cancel()
        {
            return View();
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Facility)
                .Include(b => b.FacilityInCharge)
                .Include(b => b.FacilityManager)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["FacilityID"] = new SelectList(_context.Facilities, "FacilityID", "Code");
            ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "Id");
            ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "Id");
            ViewData["UserID"] = new SelectList(_context.UserS, "Id", "Id");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,FacilityID,UserID,StartTime,EndTime,Status,FacilityManagerId,FacilityInChargeId")] Booking booking)
        {
            Console.WriteLine($"Debugging: FacilityID is {booking.FacilityID}, {booking.BookingID}");
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["success"] = "Booking successfully created.";
                
                ViewData["FacilityID"] = new SelectList(_context.Facilities, "FacilityID", "Code", booking.FacilityID);
                ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityInChargeId);
                ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityManagerId);
                ViewData["UserID"] = new SelectList(_context.UserS, "Id", "UserName", booking.UserID);

                return RedirectToAction(nameof(Index));
            }
            TempData["warning"] = "Failed to create booking.";
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["FacilityID"] = new SelectList(_context.Facilities, "FacilityID", "Code", booking.FacilityID);
            ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "UserName", booking.FacilityInChargeId);
            ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "UserName", booking.FacilityManagerId);
            ViewData["UserID"] = new SelectList(_context.UserS, "Id", "UserName", booking.UserID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,FacilityID,UserID,StartTime,EndTime,Status,FacilityManagerId,FacilityInChargeId")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                TempData["success"] = "Booking successfully updated.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Booking successfully updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        TempData["warning"] = "Booking not found.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityID"] = new SelectList(_context.Facilities, "FacilityID", "Code", booking.FacilityID);
            ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityInChargeId);
            ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityManagerId);
            ViewData["UserID"] = new SelectList(_context.UserS, "Id", "Id", booking.UserID);

            TempData["warning"] = "Failed to update booking.";
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Facility)
                .Include(b => b.FacilityInCharge)
                .Include(b => b.FacilityManager)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AppDbContext.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
    }
}
