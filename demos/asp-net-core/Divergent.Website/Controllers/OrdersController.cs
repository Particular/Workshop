using Microsoft.AspNetCore.Mvc;

namespace Divergent.Website.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index(int? pageIndex, int? pageSize)
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}