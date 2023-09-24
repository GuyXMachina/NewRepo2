using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
