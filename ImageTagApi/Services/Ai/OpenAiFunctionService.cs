using ImageTagApi.DTOs.Ai;
using System.Text.Json;
using System.Text;

namespace ImageTagApi.Services.Ai
{
    public class OpenAiFunctionService:IAiFunctionService
    {
        private readonly ILogger<OpenAiFunctionService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public OpenAiFunctionService(ILogger<OpenAiFunctionService> logger, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<AiTagResponse?> GenerateTagAsync(string aiProcessedFileUrl)
        {
            var baseUrl = _config["AzureFunctions:GenerateTagsUrl"];
            var key = _config["AzureFunctions:FunctionKey"];
            var functionUrl = $"{baseUrl}?code={key}";

            
            var requestJson = JsonSerializer.Serialize(new AiTagRequest { AiProcessedFileUrl=aiProcessedFileUrl});
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, functionUrl)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("生成に失敗しました: AiProcessedFileUrl={AiProcessedFileUrl}", aiProcessedFileUrl);
                return null;
            }
            _logger.LogInformation("生成に成功しました");

            return await response.Content.ReadFromJsonAsync<AiTagResponse>();
        }
    }
}