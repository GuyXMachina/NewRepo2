using GuitarShop.Data;
using GuitarShop.Data.DataAccess;
using GuitarShop.Models;
using GuitarShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Globalization;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

namespace GuitarShop.Controllers
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


        public IActionResult CheckOut(int id)
        {
            var facility = _repo.Facility.GetById(id);
            if (facility == null)
            {
                return NotFound();
            }

            string scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.Value;
            var domain = $"{scheme}://{host}";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "/Facility/Success",
                CancelUrl = domain + "/Facility/Cancel",
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
            TempData["BookingID"] = id;
           
            return new StatusCodeResult(303);
        }

        
        public async Task<IActionResult> Success()
        {

            try
            {
                var bookingId = TempData["BookingID"];
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking != null)
                {
                    booking.Transaction = new Transaction 
                    {
                        TransactionDate = DateTime.Now,
                        PaymentMethod = "Credit Card"
                    };
                    booking.Status = "Paid";
                    _context.Bookings.Update(booking);
                    await _context.SaveChangesAsync();
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

    }
}
