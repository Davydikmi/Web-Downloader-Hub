using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_Downloader_Hub.Models;
using Web_Downloader_Hub.Serivce;

namespace Web_Downloader_Hub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }



        // POST пост запрос на добавление элемента в список
        [HttpPost("add")]
        public IActionResult Add([FromBody] string url)
        {
            DownloadService _downloadService = new DownloadService();
            try
            {
                DownloadRecord result = _downloadService.AddToQueue(url);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST запрос на удаление элемента
        [HttpPost("/delete")]
        public IActionResult Delete([FromBody] DownloadRecord record)
        {
            DownloadService _downloadService = new DownloadService();
            try
            {
                _downloadService.DeleteFromQueue(record.filename);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/download")]
        public IActionResult DownloadFiles([FromBody] List<string> filePaths)
        {
            DownloadService _downloadService = new DownloadService();

            try
            {
                var result = _downloadService.PrepareDownload(filePaths);
                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("/clear-all")]
        public IActionResult ClearAll([FromBody] List<string> filenames)
        {
            DownloadService _downloadService = new DownloadService();
            try
            {
                _downloadService.DeleteFiles(filenames);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
