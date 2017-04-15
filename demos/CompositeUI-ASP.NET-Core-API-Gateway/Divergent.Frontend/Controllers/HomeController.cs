using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace Divergent.Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            dynamic vm = new ExpandoObject();
            vm.Foo = 10;
            vm.Bar = "Sample";

            return View(vm);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
