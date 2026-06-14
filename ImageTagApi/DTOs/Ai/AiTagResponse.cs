using System.Text.Json.Serialization;

namespace ImageTagApi.DTOs.Ai
{
    public class AiTagItem
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; } = string.Empty;
        [JsonPropertyName("bgColor")]
        public string BgColor { get; set; } = string.Empty;
    }
    public class AiTagResponse
    {
        [JsonPropertyName("items")]
        public List<AiTagItem> Items { get; set; } = new();
    }
}
