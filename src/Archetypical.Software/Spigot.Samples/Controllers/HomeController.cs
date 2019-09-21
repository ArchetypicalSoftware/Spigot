using Microsoft.AspNetCore.Mvc;

namespace Spigot.Samples.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}