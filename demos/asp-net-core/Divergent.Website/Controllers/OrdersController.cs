using Microsoft.AspNetCore.Mvc;

namespace Divergent.Website.Controllers
{
    public class OrdersController : Controller
    {
        [HttpGet("/orders")]
        public IActionResult Index(int? pageIndex, int? pageSize)
        {
            return View();
        }

        [HttpGet("/orders/details/{id}")]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}