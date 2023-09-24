using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Controllers
{
    [Authorize (Roles = "FacilityAdmin")]
    public class AdminCategoryController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        public AdminCategoryController(IRepositoryWrapper repo)
        {
            _repo = repo;
        }

        public IActionResult List()
        {
            var categories = _repo.Category.FindAll()
                .OrderBy(c => c.CategoryName).ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            return View("AddUpdate", new Category());
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Action = "Update";
            var category = _repo.Category.GetById(id);
            return View("AddUpdate", category);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.CategoryID == 0)
                {
                    _repo.Category.Create(category);
                    TempData["Message"] = $"{category.CategoryName} has been added";
                }
                else
                {
                    _repo.Category.Update(category);
                    TempData["Message"] = $"{category.CategoryName} has been updated";
                }
                _repo.Save();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
                return View("AddUpdate");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Category category = _repo.Category.GetById(id);
            return View(category);
        }


        [HttpPost]
        public IActionResult Delete(Category category)
        {
            _repo.Category.Delete(category);
            TempData["Message"] = $"{category.CategoryName} has been deleted";
            _repo.Save();
            return RedirectToAction("List");
        }
    }
}
