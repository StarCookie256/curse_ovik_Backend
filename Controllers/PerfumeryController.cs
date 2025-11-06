using Microsoft.AspNetCore.Mvc;

namespace PerfumeryBackend.Controllers
{
    public class PerfumeryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
