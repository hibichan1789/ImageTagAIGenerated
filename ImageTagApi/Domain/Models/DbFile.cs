using ImageTagApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageTagApi.Domain.Models
{
    public class DbFile
    {
        public int Id { get; set; }

        [Required]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength:255)]
        public string UniqueFileName { get; set; } = string.Empty;

        [Required]
        public string FileUrl { get; set;  } = string.Empty;
        public string? ThumbnailFileUrl { get; set;  }
        public string? AiProcessedFileUrl { get; set; }
        public long FileSizeBytes { get; set; }// 最大10MBまで

        [Required]
        [StringLength(maximumLength: 255)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength: 50)]
        public string FileExtension { get; set; } = string.Empty;

        [Required]
        public FileStatus Status { get; set; } = FileStatus.Processing;

        public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

        // User関連
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // 中間テーブル
        public ICollection<FileTag> FileTags { get; set; } = new List<FileTag>();
    }
}
