using Microsoft.AspNetCore.Mvc;

namespace Elmah2AI.SampleApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // throw an error
            var r = 0;
            var d = 100 / r;
            return View();
        }
    }
}
