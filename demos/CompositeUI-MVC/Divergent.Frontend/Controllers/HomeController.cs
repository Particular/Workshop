using System.Web.Mvc;

namespace Divergent.Frontend.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToRoute(new { controller = "Orders" });
        }
    }
}