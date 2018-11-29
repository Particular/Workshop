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
            return View(new OrderDetail
                            {
                                ShippingStatus = "Delivered",
                                OrderNumber = 1,
                                ShippingCourier = "FedEx",
                                OrderItemsCount = 2
                            });
        }
    }

    public class OrderDetail
    {
        public string ShippingStatus { get; set; }
        public string ShippingCourier { get; set; }
        public int OrderNumber { get; set; }
        public int OrderItemsCount { get; set; }
    }
}