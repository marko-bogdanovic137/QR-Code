using Microsoft.AspNetCore.Mvc;

namespace PraksaApp.Controllers
{
    public class QrCodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
