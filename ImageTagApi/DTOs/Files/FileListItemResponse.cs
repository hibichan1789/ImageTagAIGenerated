using ImageTagApi.Domain.Enums;
using ImageTagApi.DTOs.Ai;

namespace ImageTagApi.DTOs.Files
{
    public class FileListItemResponse
    {
        public int FileId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public FileStatus Status { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IEnumerable<AiTagItem> Tags { get; set; } = []; 
    }
}
