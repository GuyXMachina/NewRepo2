using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GuitarShop.Controllers
{
    [Authorize(Roles = "FacilityAdmin")]
    public class FacilityAdminController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public FacilityAdminController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        public IActionResult Facilities()
        {
            var facilities = _repoWrapper.Facility.FindAll();
            return View(facilities);
        }

        [HttpGet]
        public IActionResult CreateFacility()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                _repoWrapper.Facility.Create(model);
                _repoWrapper.Save();
                return RedirectToAction("Facilities");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateFacility(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            return View(facility);
        }

        [HttpPost]
        public IActionResult UpdateFacility(Facility model)
        {
            if (ModelState.IsValid)
            {
                _repoWrapper.Facility.Update(model);
                _repoWrapper.Save();
                return RedirectToAction("Facilities");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteFacility(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            return View(facility);
        }

        [HttpPost]
        public IActionResult DeleteFacilityConfirmed(int id)
        {
            var facility = _repoWrapper.Facility.GetById(id);
            if (facility != null)
            {
                _repoWrapper.Facility.Delete(facility);
                _repoWrapper.Save();
            }
            return RedirectToAction("Facilities");
        }

        public IActionResult FacilityManagers()
        {
            var managers = _repoWrapper.FacilityManager.FindAll();
            return View(managers);
        }

        [HttpGet]
        public IActionResult CreateFacilityManager()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFacilityManager(FacilityManager model)
        {
            if (ModelState.IsValid)
            {
                var newManager = new FacilityManager
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserType = UserType.Staff,
                    EmployeeID = model.Email,
                    Id = $"{model.Email.Trim()} {model.Name.Trim()}{model.Surname.Trim()}"
                };

                _repoWrapper.FacilityManager.Create(model);
                _repoWrapper.Save();
                return RedirectToAction("FacilityManagers");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult UpdateFacilityManager(string id)
        {
            var manager = _repoWrapper.FacilityManager.GetById(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }

        [HttpPost]
        public IActionResult UpdateFacilityManager(FacilityManager model)
        {
            if (ModelState.IsValid)
            {
                _repoWrapper.FacilityManager.Update(model);
                _repoWrapper.Save();
                return RedirectToAction("FacilityManagers");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteFacilityManager(string id)
        {
            var manager = _repoWrapper.FacilityManager.GetById(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }

        [HttpPost]
        public IActionResult DeleteFacilityManagerConfirmed(FacilityManager manager)
        {
            if (manager != null)
            {
                _repoWrapper.FacilityManager.Delete(manager);
                _repoWrapper.Save();
                TempData["Message"] = $"{manager.Name} {manager.Surname} has been deleted";
            }
            return RedirectToAction("FacilityManagers");
        }

    }

}
