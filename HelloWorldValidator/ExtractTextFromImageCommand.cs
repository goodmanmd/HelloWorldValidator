using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelloWorldValidator
{
    public class TextExtractionResult
    {
        public string Text { get; set; }
        public string Error { get; set; }
    }

    public class ExtractTextFromImageCommand
    {
        private ExtractTextFromImageApiClient _apiClient;

        public ExtractTextFromImageCommand(ExtractTextFromImageApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<TextExtractionResult> ExtractText(string imageUrl)
        {
            var apiResponse = await _apiClient.CallApi(imageUrl);
            var result = ProcessApiResponse(apiResponse);

            return result;
        }

        private TextExtractionResult ProcessApiResponse(ApiResponse response)
        {
            if (response == null)
            {
                return new TextExtractionResult
                {
                    Error = "Failed to process image"
                };
            }

            if (!string.IsNullOrEmpty(response.Message))
            {
                return new TextExtractionResult
                {
                    Error = response.Message
                };
            }

            var text = string.Join("\r\n", response.Regions.Select(r => string.Join("\r\n", r.Lines.Select(l => string.Join(" ", l.Words.Select(w => w.Text))))));

            return new TextExtractionResult
            {
                Text = text
            };
        }
    }

    public class ExtractTextFromImageApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExtractTextFromImageApiClient> _logger;

        public ExtractTextFromImageApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ExtractTextFromImageApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse> CallApi(string imageUrl)
        {
            try
            {
                var apiEndpoint = _configuration["VisionApi:ApiEndpoint"];
                var requestParams = "language=en&detectOrientation=true";
                var requestUri = $"{apiEndpoint}?{requestParams}";

                var httpClient = _httpClientFactory.CreateClient<ExtractTextFromImageApiClient>();

                var response = await httpClient.PostAsJsonAsync(requestUri, new { url = imageUrl });
                return await response.Content.ReadAsAsync<ApiResponse>();
            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred calling Vision API: {ex.ToString()}");
                throw;
            }
        }
    }

    public class ApiResponse
    {
        public string Language { get; set; }
        public float TextAngle { get; set; }
        public string Orientation { get; set; }
        public List<ImageRegion> Regions { get; set; }

        public string Code { get; set; }
        public string RequestId { get; set; }
        public string Message { get; set; }
    }

    public class ImageRegion
    {
        public string BoundingBox { get; set; }
        public List<ImageLine> Lines { get; set; }
    }

    public class ImageLine
    {
        public string BoundingBox { get; set; }
        public List<ImageWords> Words { get; set; }
    }

    public class ImageWords
    {
        public string BoundingBox { get; set; }
        public string Text { get; set; }
    }
}
