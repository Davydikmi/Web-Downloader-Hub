using Microsoft.AspNetCore.Mvc;
using Web_Downloader_Hub.Serivce;

namespace Web_Downloader_Hub.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("history/get-all")]
        public IActionResult GetAll()
        {
            HistoryStatService _historyService = new HistoryStatService();
            return Json(_historyService.DownloadRecords);
        }
    }
}
