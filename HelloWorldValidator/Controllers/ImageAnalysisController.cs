using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HelloWorldValidator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelloWorldValidator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageAnalysisController : ControllerBase
    {
        private ExtractTextFromImageCommand _extractTextFromImageCommand;
        private ILogger<ImageAnalysisController> _logger;

        public ImageAnalysisController(ExtractTextFromImageCommand extractTextFromImageCommand, ILogger<ImageAnalysisController> logger)
        {
            _extractTextFromImageCommand = extractTextFromImageCommand;
            _logger = logger;
        }

        private static Regex _helloWorldRegex = new Regex(@"hello(,)?(\s+)world", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [HttpPost]
        public async Task<ActionResult<ExtractionResultModel>> ContainsHelloWorld([FromBody] HelloWorldRequestModel model)
        {
            try
            {                
                var extractionResult = await _extractTextFromImageCommand.ExtractText(model.ImageUrl);
                return new ExtractionResultModel
                {
                    ContainsHelloWorld = _helloWorldRegex.IsMatch(extractionResult?.Text ?? ""),
                    ErrorMessage = extractionResult?.Error
                };
            }
            
            catch (Exception e)
            {
                _logger.LogError($"Error attempting to extract text from {model.ImageUrl}: {e.Message}");
                return new ExtractionResultModel
                {
                    ContainsHelloWorld = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}