using Microsoft.AspNetCore.Mvc;

namespace Spigot.Samples.Areas.EventualConsistency.Controllers
{
    [Area("EventualConsistency")]
    public class MaterializedViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}