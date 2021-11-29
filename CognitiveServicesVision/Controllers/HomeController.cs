using CognitiveServicesVision.Models;
using CognitiveServicesVision.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServicesVision.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReadTextInImageService _readTextInImageService;

        public HomeController(ILogger<HomeController> logger, IReadTextInImageService readTextInImageService)
        {
            _logger = logger;
            _readTextInImageService = readTextInImageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("home/UploadWebCamImage")]
        public async Task<IActionResult> UploadWebCamImage([FromBody] FileUploadModel fileUploadModel)
        {
            var inputPath = _readTextInImageService.SaveImageTemp(fileUploadModel.ImageData);
            var outputPath = new ReadTextInImageModel();

            if(fileUploadModel.Type == "ocr")
            {
                outputPath = await _readTextInImageService.GetTextOcrAsync(inputPath);
            }                
            else
            {
                outputPath = await _readTextInImageService.GetTextReadAsync(inputPath);
            }          


            HttpContext.Session.SetString("ReadTextInImageSession", JsonConvert.SerializeObject(outputPath) );
            var redirectUrl = Request.Scheme + "://" + Request.Host + "/home/imagetoText";

            return Json(new { url = redirectUrl });
        }

        [HttpGet("home/imagetoText")]
        public IActionResult ImageToText()
        {
            var sessionData = HttpContext.Session.GetString("ReadTextInImageSession");
            var outputPath = JsonConvert.DeserializeObject<ReadTextInImageModel>(sessionData);

            return View(outputPath);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
