using System.Web.Mvc;

namespace Divergent.Frontend.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Details", "Orders", new { id = 1 });
        }
    }
}