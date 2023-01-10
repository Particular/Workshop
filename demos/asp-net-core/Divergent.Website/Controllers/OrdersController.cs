using Microsoft.AspNetCore.Mvc;

namespace Divergent.Website.Controllers
{
    public class OrdersController : Controller
    {
        [HttpGet("/orders")]
        public IActionResult Index(int? pageIndex, int? pageSize)
        {
            /*
             * Pagination is not used. It's kept in the code
             * to demonsrate:
             * - how to propagatr it down to composition handlers
             * - that it's owned by Sales
             */
            return View();
        }

        [HttpGet("/orders/details/{id}")]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}