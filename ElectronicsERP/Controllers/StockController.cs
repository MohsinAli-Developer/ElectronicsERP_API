using Microsoft.AspNetCore.Mvc;

namespace ElectronicsERP.Controllers
{
    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
