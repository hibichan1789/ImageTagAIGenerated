using ImageTagApi.Domain.Models;
using ImageTagApi.Domain.Enums;
using ImageTagApi.DTOs.Files;
using ImageTagApi.Services.Queue;

namespace ImageTagApi.Services.Files
{
    public class FileService:IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IFileStorageService _storage;
        private readonly IFileRepository _repository;
        private readonly IQueueService _queue;

        // あとからファイル許容サイズの変更を変えれる
        private static readonly int _maxImageSizeMb = 10;
        private static readonly long _maxImageSize = _maxImageSizeMb * 1024 * 1024;
        private static readonly string[] _validContentTypes =
        {
            "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml", "image/avif"
        };
        private static readonly string[] _validExtensions =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".avif"
        };

        public FileService(
                ILogger<FileService> logger,
                IFileStorageService storage,
                IFileRepository repository,
                IQueueService queue
            )
        {
            _logger = logger;
            _storage = storage;
            _repository = repository;
            _queue = queue;
        }

        public async Task<FileUploadResponse> UploadAsync(IFormFile file, int userId)
        {
            _logger.LogInformation("UploadAsync 開始, userId={userId}", userId);

            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("ファイルが無効です");
            }

            if(!ValidateFileSize(file.Length, _maxImageSize))
            {
                _logger.LogWarning("Warning: Invalid FileSize={FileSize}", file.Length/(1024*1024));
                throw new ArgumentException($"ファイルサイズは{_maxImageSizeMb}MB以下にしてください");
            }

            if (!ValidateContentType(file.ContentType))
            {
                _logger.LogWarning("Warning: Invalid ContentType={ContentType}", file.ContentType);
                throw new ArgumentException("ファイル形式が不正です");
            }

            var ext = Path.GetExtension(file.FileName);
            if (!ValidateExtension(ext))
            {
                _logger.LogWarning("Warning: Invalid Extension={Extension}", ext);
                throw new ArgumentException("拡張子が不正です");
            }

            var storageResult = await _storage.SaveAsync(file);

            var dbFile = new DbFile
            {
                UserId = userId,
                OriginalFileName = file.FileName,
                UniqueFileName = storageResult.UniqueFileName,
                FileUrl = storageResult.FileUrl,
                FileExtension = storageResult.FileExtension,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                Status = FileStatus.Processing,
                UploadedAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(dbFile);

            await _repository.SaveChangesAsync();
            _logger.LogInformation("DB登録完了: fileId={fileId}", dbFile.Id);

            await _queue.SendFileProcessRequestAsync(dbFile.Id);

            return new FileUploadResponse
            {
                FileId = dbFile.Id,
                ThumbnailUrl = null,
                OriginalFileName = dbFile.OriginalFileName,
                FileStatus = dbFile.Status
            };
        }

        private static bool ValidateFileSize(long fileSize, long maxFileSize)
        {
            return fileSize <= maxFileSize;
        }

        private static bool ValidateContentType(string contentType)
        {
            return _validContentTypes.Contains(contentType.ToLower());
        }

        private static bool ValidateExtension(string extension)
        {
            return _validExtensions.Contains(extension.ToLower());
        }

        // MIME sniffingの実装も後から追加してよりセキュアにしたい
    }
}
