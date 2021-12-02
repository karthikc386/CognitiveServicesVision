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
        private readonly IAnalyzingImageService _analyzingImageService;

        public HomeController(ILogger<HomeController> logger, IReadTextInImageService readTextInImageService, IAnalyzingImageService analyzingImageService)
        {
            _logger = logger;
            _readTextInImageService = readTextInImageService;
            _analyzingImageService = analyzingImageService;
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
            var type = string.Empty;
            var redirectUrl = string.Empty;

            if (fileUploadModel.Type == "ocr")
            {
                outputPath = await _readTextInImageService.GetTextOcrAsync(inputPath);
                type = "text"; 
            }                
            else if (fileUploadModel.Type == "text")
            {
                outputPath = await _readTextInImageService.GetTextReadAsync(inputPath);
                type = "text";
            }          
            else
            {
                type = "analyzing";

                var imageAnalysisModel = await _analyzingImageService.AnalyzeImageAsync(inputPath);
                HttpContext.Session.SetString("AnalyzingImageSession", JsonConvert.SerializeObject(imageAnalysisModel));
                HttpContext.Session.SetString("InputImageSession", inputPath);
                redirectUrl = Request.Scheme + "://" + Request.Host + "/home/analyzingimage";
            }

            if(type == "text")
            {
                HttpContext.Session.SetString("ReadTextInImageSession", JsonConvert.SerializeObject(outputPath));
                redirectUrl = Request.Scheme + "://" + Request.Host + "/home/imagetoText";
            }
            

            return Json(new { url = redirectUrl });
        }

        [HttpGet("home/imagetoText")]
        public IActionResult ImageToText()
        {
            var sessionData = HttpContext.Session.GetString("ReadTextInImageSession");
            var readTextInImageModel = JsonConvert.DeserializeObject<ReadTextInImageModel>(sessionData);

            return View(readTextInImageModel);
        }

        [HttpGet("home/analyzingimage")]
        public IActionResult AnalyzingImage()
        {
            var sessionData = HttpContext.Session.GetString("AnalyzingImageSession");
            ViewData["ImageUrl"] = HttpContext.Session.GetString("InputImageSession");
            var imageAnalysisModel = JsonConvert.DeserializeObject<ImageAnalysisModel>(sessionData);

            return View(imageAnalysisModel);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
