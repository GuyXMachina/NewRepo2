using GuitarShop.Data;
using GuitarShop.Models;
using GuitarShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Controllers
{
    [Authorize (Roles = "FacilityAdmin")]
    public class AdminFacilityController : Controller
    {
        private IEnumerable<Category> _categories;
        private readonly IRepositoryWrapper _repo;

        public AdminFacilityController(IRepositoryWrapper repo)
        {
            _repo = repo;
            _categories = _repo.Category.FindAll()
                .OrderBy(c => c.CategoryName);
        }

        public IActionResult List(string id = "all")
        {
            IEnumerable<Facility> facility;
            if (id == "all")
            {
                facility = _repo.Facility.FindAll()
                    .OrderBy(p => p.Name);
            }
            else
            {
                facility = _repo.Facility.GetFacilitiesWithBookings()
                    .Where(p => p.Category.CategoryName.ToLower() == id.ToLower())
                    .OrderBy(p => p.Name);
            }

            var model = new FacilityListViewModel
            {
                //Categories = _categories,
                Facilities = facility,
                SelectedCategory = id
            };

            // bind products to view
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // create new Product object
            Facility facility = new();

            // add Category object - prevents validation problem
            facility.Category = _repo.Category.GetById(1);  

            // use ViewBag to pass action and category data to view
            ViewBag.Action = "Add";
            ViewBag.Categories = _categories;

            // bind product to AddUpdate view
            return View("AddUpdate", facility);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            // get Product object for specified primary key
            Facility facility = _repo.Facility.GetById(id);

            // use ViewBag to pass action and category data to view
            ViewBag.Action = "Update";
            ViewBag.Categories = _categories;

            // bind product to AddUpdate view
            return View("AddUpdate", facility);
        }

        [HttpPost]
        public IActionResult Update(Facility facility)
        {
            if (ModelState.IsValid)
            {
                if (facility.FacilityID == 0) // new product
                {
                    _repo.Facility.Create(facility);
                    TempData["Message"] = $"{facility.Name} has been added";
                }
                else  // existing product
                {
                    _repo.Facility.Update(facility);
                    TempData["Message"] = $"{facility.Name} has been updated";
                }
                _repo.Save();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
                ViewBag.Categories = _categories;
                return View("AddUpdate", facility);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Facility facility = _repo.Facility.GetById(id);
            return View(facility);
        }

        [HttpPost]
        public IActionResult Delete(Facility facility)
        {
            _repo.Facility.Delete(facility);
            TempData["Message"] = $"{facility.Name} has been deleted";
            _repo.Save();
            return RedirectToAction("List");
        }
    }
}
