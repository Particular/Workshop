using Microsoft.AspNetCore.Mvc;

namespace Divergent.Website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToRoute(new { controller = "Orders" });
        }
    }
}
