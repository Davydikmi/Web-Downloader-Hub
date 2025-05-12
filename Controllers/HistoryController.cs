using Microsoft.AspNetCore.Mvc;

namespace Web_Downloader_Hub.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
