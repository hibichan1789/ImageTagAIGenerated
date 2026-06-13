using ImageTagApi.DTOs.Files;

namespace ImageTagApi.Services.Files
{
    public interface IFileStorageService
    {
        Task<FileStorageResponse> SaveAsync(IFormFile file);
    }
}
