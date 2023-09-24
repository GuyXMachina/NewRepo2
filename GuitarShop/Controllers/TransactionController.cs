﻿using GuitarShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GuitarShop.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly IRepositoryWrapper _repo;

        public TransactionController(IRepositoryWrapper repo)
        {
            _repo = repo;
        }

        // For Users: View all transactions
        public IActionResult Index()
        {
            var transactions = _repo.Transaction.FindAll();  // Assuming FindAll() returns all transactions
            return View(transactions);
        }

        // For Facility Admin: View and manage all transactions
        [Authorize(Roles = "FacilityAdmin")]
        public IActionResult ManageTransactions()
        {
            var transactions = _repo.Transaction.FindAll();
            return View(transactions);
        }

        // View Transaction Details
        public IActionResult Details(int id)
        {
            var transaction = _repo.Transaction.GetById(id);  // Assuming GetById() fetches by TransactionID
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // Facility Manager: View related transactions
        [Authorize(Roles = "FacilityManager")]
        public IActionResult FacilityTransactions()
        {
            // Fetch transactions related to facilities managed by this manager
            var transactions = _repo.Transaction.GetByFacilityManager(User.Identity.Name); // Hypothetical method
            return View(transactions);
        }

        // For Facility In-charge: View bookings they are in charge of
        [Authorize(Roles = "FacilityInCharge")]
        public IActionResult InChargeTransactions()
        {
            // Fetch transactions related to bookings this in-charge is responsible for
            var transactions = _repo.Transaction.GetByInCharge(User.Identity.Name); // Hypothetical method
            return View(transactions);
        }
    }
}