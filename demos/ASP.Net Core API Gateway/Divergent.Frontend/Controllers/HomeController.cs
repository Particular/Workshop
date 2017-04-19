using Microsoft.AspNetCore.Mvc;

namespace Divergent.Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToRoute(new { controller = "Orders" });
        }
    }
}
