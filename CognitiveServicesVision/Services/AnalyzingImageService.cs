using CognitiveServicesVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServicesVision.Services
{
    public interface IAnalyzingImageService
    {
        Task<ImageAnalysisModel> AnalyzeImageAsync(string imageFile);
    }

    public class AnalyzingImageService : IAnalyzingImageService
    {
        private readonly ComputerVisionClient _computerVisionClient;
        private readonly List<VisualFeatureTypes?> _features = new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Brands,
            VisualFeatureTypes.Objects,
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.Color,
            VisualFeatureTypes.ImageType
        };

        public AnalyzingImageService(IComputerVisionClientService computerVisionClientService)
        {
            _computerVisionClient = computerVisionClientService.GetComputerVisionClient();
        }

        public async Task<ImageAnalysisModel> AnalyzeImageAsync(string imageFile)
        {
            var imageAnalysisModel = new ImageAnalysisModel();

            try
            {
                // Get image analysis
                using (var imageData = File.OpenRead($"wwwroot/{imageFile}"))
                {
                    imageAnalysisModel.ImageAnalysisResult = await _computerVisionClient.AnalyzeImageInStreamAsync(imageData, _features);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetTextOcrAsync" + ex.Message + ex.StackTrace);
            }

            return imageAnalysisModel;
        }
    }
}
