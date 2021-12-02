using CognitiveServicesVision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Options;

namespace CognitiveServicesVision.Services
{
    public interface IComputerVisionClientService
    {
        ComputerVisionClient GetComputerVisionClient();
    }

    public class ComputerVisionClientService : IComputerVisionClientService
    {
        private readonly CogSvcModel _settings;

        public ComputerVisionClientService(IWebHostEnvironment webHostEnvironment,
            IOptions<CogSvcModel> settings)
        {
            _settings = settings.Value;
        }

        public ComputerVisionClient GetComputerVisionClient()
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_settings.Key);
            return new ComputerVisionClient(credentials)
            {
                Endpoint = _settings.Endpoint
            };
        }
    }
}
