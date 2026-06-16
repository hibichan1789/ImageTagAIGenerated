using ImageTagApi.DTOs.Files;

namespace ImageTagApi.Services.Files
{
    public class LocalFileStorageService:IFileStorageService
    {
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(ILogger<LocalFileStorageService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task<FileStorageResponse> SaveAsync(IFormFile file)
        {
            var uploadDir = Path.Combine(_env.WebRootPath,"images","originals");

            if (!Directory.Exists(uploadDir))
            {
                _logger.LogInformation("uploadDir: {uploadDir}が存在しないため作成します", uploadDir);
                Directory.CreateDirectory(uploadDir);
            }

            var ext = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";

            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("filePath: {filePath}に保存されました", filePath);

            // 公開URL(フロントがアクセスできる)
            var fileUrl = $"/images/originals/{uniqueFileName}";

            // FileServiceがDBのRepositoryにサービス渡すためにuniqueFileNameとfileUrlとextを返す
            return new FileStorageResponse
            {
                UniqueFileName = uniqueFileName,
                FileUrl = fileUrl,
                FileExtension = ext
            };
        }

        public string GenerateSasUrl(string thumbnailUrl, int mituteExpired = 5)
        {
            return thumbnailUrl;
        }
    }
}
