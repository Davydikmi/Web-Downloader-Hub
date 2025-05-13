using Microsoft.AspNetCore.Mvc;
using Web_Downloader_Hub.Models;
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

        [HttpPost("/history/delete")]
        public IActionResult DeleteFromHistory([FromBody] DownloadRecord record)
        {
            if (record == null)
                return BadRequest("Record is null");

            try
            {
                DownloadService _downloadService = new DownloadService();
                _downloadService.DeleteFromQueue(record);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении из истории: {ex.Message}");
            }
        }

    }
}
