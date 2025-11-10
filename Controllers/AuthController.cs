using Microsoft.AspNetCore.Mvc;

namespace PerfumeryBackend.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
