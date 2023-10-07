﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GuitarShop.Data;
using GuitarShop.Models;

namespace GuitarShop
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
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityID"] = new SelectList(_context.Facilities, "FacilityID", "Code", booking.FacilityID);
            ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityInChargeId);
            ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityManagerId);
            ViewData["UserID"] = new SelectList(_context.UserS, "Id", "Id", booking.UserID);
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
            ViewData["FacilityInChargeId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityInChargeId);
            ViewData["FacilityManagerId"] = new SelectList(_context.UserS, "Id", "Id", booking.FacilityManagerId);
            ViewData["UserID"] = new SelectList(_context.UserS, "Id", "Id", booking.UserID);
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
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
