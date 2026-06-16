using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using ImageTagApi.DTOs.Files;
using Azure.Storage.Sas;
namespace ImageTagApi.Services.Files
{
    public class AzureBlobStorageService:IFileStorageService
    {
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(
            ILogger<AzureBlobStorageService> logger,
            IConfiguration config
            )
        {
            _logger = logger;

            var conn = config["AzureStorage:ConnectionString"];
            var containerName = config["AzureStorage:ContainerName"];

            _containerClient = new BlobContainerClient(conn, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<FileStorageResponse> SaveAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";

            var blobPath = $"originals/{uniqueFileName}";
            var blob = _containerClient.GetBlobClient(blobPath);


            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            

            // 公開URL(フロントがアクセスできる)
            var fileUrl = blob.Uri;
            _logger.LogInformation("ファイルが保存されました: URL={Url}", blob.Uri);

            // FileServiceがDBのRepositoryにサービス渡すためにuniqueFileNameとfileUrlとextを返す
            return new FileStorageResponse
            {
                UniqueFileName = uniqueFileName,
                FileUrl = fileUrl.ToString(),
                FileExtension = ext
            };
        }

        public string GenerateSasUrl(string thumbnailUrl, int mituteExpired = 10)
        {
            var blobPath = GetBlobPathFromUrl(thumbnailUrl);

            
            var blobClient = _containerClient.GetBlobClient(blobPath);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = blobPath,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(mituteExpired)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUrl = blobClient.GenerateSasUri(sasBuilder);

            return sasUrl.ToString();
        }

        private string GetBlobPathFromUrl(string url)
        {
            var localPath = new Uri(url).LocalPath;
            var path = localPath.TrimStart('/');

            if(path.StartsWith(_containerClient.Name + "/"))
            {
                path = path.Substring(_containerClient.Name.Length + 1);
            }

            return path;
        }
    }
}
