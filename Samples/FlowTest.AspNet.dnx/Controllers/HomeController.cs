using Microsoft.AspNet.Mvc;

namespace FlowTest.AspNet.dnx.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
