using System.Text.Json.Serialization;

namespace ImageTagApi.DTOs.Ai
{
    public class AiTagRequest
    {
        [JsonPropertyName("ai_processed_file_url")]
        public string AiProcessedFileUrl { get; set; } = string.Empty;
    }
}
