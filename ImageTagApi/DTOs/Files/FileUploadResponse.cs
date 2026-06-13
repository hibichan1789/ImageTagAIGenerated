using ImageTagApi.Domain.Enums;

namespace ImageTagApi.DTOs.Files
{
    public class FileUploadResponse
    {
        public int FileId { get; set; }

        // 画面表示用
        public string OriginalFileName { get; set; } = string.Empty;
        public FileStatus FileStatus { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
