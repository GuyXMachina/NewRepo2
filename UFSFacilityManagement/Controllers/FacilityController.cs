﻿using UFSFacilityManagement.Data;
using UFSFacilityManagement.Data.DataAccess;
using UFSFacilityManagement.Models;
using UFSFacilityManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Globalization;
using System.Linq.Expressions;

namespace UFSFacilityManagement.Controllers
{
    public class FacilityController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        private readonly AppDbContext _context;

        public FacilityController(IRepositoryWrapper repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }
        public int iPageSize = 4;
        public IActionResult List(string id = "all", string sortBy = "name", int Page = 1)
        {
            IEnumerable<Facility> products;
            Expression<Func<Facility, object>> orderBy;
            string orderByDirection;
            int iTotalProducts;

            ViewData["NameSortParam"] = sortBy == "name" ? "name_desc" : "name";
            ViewData["PriceSortParam"] = sortBy == "price" ? "price_desc" : "price";

            if (sortBy.EndsWith("_desc"))
            {
                sortBy = sortBy.Substring(0, sortBy.Length - 5);
                orderByDirection = "desc";
            }
            else
            {
                orderByDirection = "asc";
            }

            string sPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sortBy);
            orderBy = p => EF.Property<object>(p, sPropertyName);  //e.g. p => p.Name

            if (id == "all")
            {
                iTotalProducts = _repo.Facility.FindAll().Count();
                products = _repo.Facility.GetWithOptions(new QueryOptions<Facility>
                {
                    OrderBy = orderBy,
                    OrderByDirection = orderByDirection,
                    PageNumber = Page,
                    PageSize = iPageSize
                });
            }
            else
            {
                var category = _repo.Category.FindByCondition(c => c.CategoryName.ToLower() == id).FirstOrDefault();

                if (category == null)
                {
                    // Handle the case where no category was found. 
                    // You could log an error, throw an exception, or redirect the user.
                    // For now, let's redirect to the list of all facilities.
                    return RedirectToAction("List", new { id = "all" });
                }

                int iCatId = category.CategoryID;

                iTotalProducts = _repo.Facility.FindByCondition(p => p.Category.CategoryID == iCatId).Count();
                products = _repo.Facility.GetWithOptions(new QueryOptions<Facility>
                {
                    OrderBy = orderBy,
                    OrderByDirection = orderByDirection,
                    Where = p => p.Category.CategoryID == iCatId,
                    PageNumber = Page,
                    PageSize = iPageSize
                });
            }

            var model = new FacilityListViewModel
            {
                Facilities = products,
                SelectedCategory = id,
                PagingInfo = new PagingInfoViewModel
                {
                    CurrentPage = Page,
                    ItemsPerPage = iPageSize,
                    TotalItems = iTotalProducts
                }
            };
            
           return View(model);
        }
        public IActionResult Details(int id)
        {
            Facility product = _repo.Facility.GetById(id);
            if (product != null)
            {
                // use ViewBag to pass data to view
                ViewBag.ImageFilename = product.Code + "_m.png";

                // bind product to view
                return View(product);
            }
            else
                return RedirectToAction("List");
        }
        public IActionResult Reviews()
        {
            List<Models.Review> reviews = _context.Reviews
                  .Include(r => r.User)
                  .ToList();

            return View(reviews);
        }

        [HttpPost]
        public IActionResult AddReview(Models.Review newReview)
        {
            if (ModelState.IsValid)
            {
                // Add the review to the database
                _context.Reviews.Add(newReview);
                _context.SaveChanges();
                return RedirectToAction("Reviews");
            }
            return View(newReview);
        }

    }
}
