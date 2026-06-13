using System.Text.Json.Serialization;

namespace ImageTagApi.DTOs.Files
{
    public class FileProcessRequest
    {
        [JsonPropertyName("file_id")]
        public int FileId { get; set; }
    }
}
