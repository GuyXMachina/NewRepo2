using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
