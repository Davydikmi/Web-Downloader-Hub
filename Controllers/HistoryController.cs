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

        [HttpPost("/history/repeat")]
        public IActionResult RepeatDownloadMeta([FromBody] DownloadRecord record)
        {
            if (record == null || string.IsNullOrEmpty(record.url))
                return BadRequest("Недопустимая запись.");

            record.Id = Guid.NewGuid();
            record.DownloadDate = DateTime.UtcNow;

            try
            {
                HistoryStatService historyStatService = new HistoryStatService();
                historyStatService.AddRecord(record);

                return Ok(record); // отправляем обновлённую запись обратно
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/history/download")]
        public IActionResult DownloadFile([FromQuery] string filepath, [FromQuery] string filename)
        {
            DownloadService _downloadService = new DownloadService();
            var result = _downloadService.PrepareDownload(new List<string>() { filepath });
            return File(result.Data, result.ContentType, filename);
        }


    }
}
