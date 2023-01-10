using Microsoft.AspNetCore.Mvc;

namespace Divergent.Website.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return RedirectToRoute(new { controller = "Orders" });
        }
    }
}
