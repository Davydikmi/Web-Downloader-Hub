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



        // POST Request to get information about url
        [HttpPost("add")]
        public IActionResult Add([FromBody] string url)
        {
            DownloadService _downloadService  = new DownloadService();
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
        public IActionResult Index()
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
