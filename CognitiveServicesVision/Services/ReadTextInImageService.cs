using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CognitiveServicesVision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;

namespace CognitiveServicesVision.Services
{
    public interface IReadTextInImageService
    {
        string SaveImageTemp(string imageData);
        Task<ReadTextInImageModel> GetTextOcrAsync(string imageFile);
        Task<ReadTextInImageModel> GetTextReadAsync(string imageFile);        
    }

    public class ReadTextInImageService : IReadTextInImageService
    {
        private readonly ComputerVisionClient _computerVisionClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReadTextInImageService(IWebHostEnvironment webHostEnvironment, 
            IComputerVisionClientService computerVisionClientService)
        {
            _computerVisionClient = computerVisionClientService.GetComputerVisionClient();
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ReadTextInImageModel> GetTextOcrAsync(string imageFile)
        {
            var readText = new ReadTextInImageModel();
            readText.InputUrl = imageFile;

            try
            {
                using (var imageData = File.OpenRead($"wwwroot/{imageFile}"))
                {
                    var ocrResults = await _computerVisionClient.RecognizePrintedTextInStreamAsync(detectOrientation: false, image: imageData);

                    // Prepare image for drawing
                    Image image = Image.FromFile($"wwwroot/{imageFile}");
                    Graphics graphics = Graphics.FromImage(image);
                    Pen pen = new Pen(Color.Magenta, 3);

                    foreach (var region in ocrResults.Regions)
                    {
                        foreach (var line in region.Lines)
                        {
                            // Show the position of the line of text
                            int[] dims = line.BoundingBox.Split(",").Select(int.Parse).ToArray();
                            Rectangle rect = new Rectangle(dims[0], dims[1], dims[2], dims[3]);
                            graphics.DrawRectangle(pen, rect);

                            // Read the words in the line of text
                            string lineText = "";
                            foreach (var word in line.Words)
                            {
                                lineText += "<p>";
                                lineText += word.Text + " ";
                                lineText += "</p>";
                            }

                            readText.TextInImage += lineText.Trim();
                            Console.WriteLine(lineText.Trim());
                        }
                    }

                    // Save the image with the text locations highlighted
                    String output_file = $"wwwroot/{imageFile.Replace("InputFile", "ocr_results")}";
                    
                    image.Save(output_file);
                    Console.WriteLine("Results saved in " + output_file);

                    
                    readText.OutputUrl = imageFile.Replace("InputFile", "ocr_results");
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetTextOcrAsync" + ex.Message + ex.StackTrace);
            }

            return readText;
        }



        public async Task<ReadTextInImageModel> GetTextReadAsync(string imageFile)
        {
            var readText = new ReadTextInImageModel();
            readText.InputUrl = imageFile;

            try
            {
                // Use Read API to read text in image
                using (var imageData = File.OpenRead($"wwwroot/{imageFile}"))
                {
                    var readOp = await _computerVisionClient.ReadInStreamAsync(imageData);

                    // Get the async operation ID so we can check for the results
                    string operationLocation = readOp.OperationLocation;
                    string operationId = operationLocation.Substring(operationLocation.Length - 36);

                    // Wait for the asynchronous operation to complete
                    ReadOperationResult results;
                    do
                    {
                        Thread.Sleep(1000);
                        results = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));
                    }
                    while ((results.Status == OperationStatusCodes.Running ||
                            results.Status == OperationStatusCodes.NotStarted));

                    // If the operation was successfuly, process the text line by line
                    if (results.Status == OperationStatusCodes.Succeeded)
                    {
                        var textUrlFileResults = results.AnalyzeResult.ReadResults;
                        string lineText = "";
                        foreach (ReadResult page in textUrlFileResults)
                        {
                            foreach (Line line in page.Lines)
                            {
                                Console.WriteLine(line.Text);
                                lineText += "<p>";
                                lineText += line.Text + " ";
                                lineText += "</p>";
                            }
                        }

                        readText.TextInImage = lineText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetTextReadAsync" + ex.Message + ex.StackTrace);
            }

            return readText;
        }

        public string SaveImageTemp (string imageData)
        {
            string newFolderName = $"images\\{Guid.NewGuid()}";
            string folderName = Path.Combine(_webHostEnvironment.WebRootPath, newFolderName);

            Directory.CreateDirectory(folderName);

            var filename = "InputFile.png";

            // Use Combine again to add the file name to the path.
            var filePathString = Path.Combine(folderName, filename);

            // Verify the path that you have constructed.
            Console.WriteLine("Path to my file: {0}\n", filePathString);

            File.WriteAllBytes(filePathString, Convert.FromBase64String(imageData));

            return Path.Combine(newFolderName, filename);
        }
    }
}
