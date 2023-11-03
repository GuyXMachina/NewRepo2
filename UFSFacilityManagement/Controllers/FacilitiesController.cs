using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UFSFacilityManagement.Data;
using UFSFacilityManagement.Models;

namespace UFSFacilityManagement.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly AppDbContext _context;

        public FacilitiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Facilities.Include(f => f.Category);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facilities == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            var facility = await _context.Facilities
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FacilityID == id);
            if (facility == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            return View(facility);
        }


        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityID,Picture,Code,Name,Price,PricingType,DiscountPercent,CategoryID,Location,Capacity")] Facility facility)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
            return View(facility);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facilities == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacilityID,Picture,Code,Name,Price,PricingType,DiscountPercent,CategoryID,Location,Capacity,AvailabilityTimes")] Facility facility)
        {
            if (id != facility.FacilityID)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facility.FacilityID))
                    {
                        TempData["warning"] = "Facility not found.";
                        return View();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", facility.CategoryID);
            return View(facility);
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Facilities == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            var facility = await _context.Facilities
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FacilityID == id);
            if (facility == null)
            {
                TempData["warning"] = "Facility not found.";
                return View();
            }

            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facilities == null)
            {
                return Problem("Entity set 'AppDbContext.Facilities'  is null.");
            }
            var facility = await _context.Facilities.FindAsync(id);
            if (facility != null)
            {
                _context.Facilities.Remove(facility);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return _context.Facilities.Any(e => e.FacilityID == id);
        }
    }
}
