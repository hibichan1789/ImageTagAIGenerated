using ImageTagApi.DTOs.Files;

namespace ImageTagApi.Services.Files
{
    public interface IFileStorageService
    {
        Task<FileStorageResponse> SaveAsync(IFormFile file);
        string GenerateSasUrl(string thumbnailUrl, int mituteExpired = 5);
    }
}
