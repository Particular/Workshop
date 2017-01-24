using System.Web.Mvc;

namespace Divergent.Frontend.Controllers
{
    public class OrdersController : Controller
    {
        //[Compose]
        public ActionResult Index(int? pageIndex, int? pageSize)
        {
            return View();
        }

        //[Compose]
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}