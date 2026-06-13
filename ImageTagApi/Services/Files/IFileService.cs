using ImageTagApi.DTOs.Files;

namespace ImageTagApi.Services.Files
{
    public interface IFileService
    {
        Task<FileUploadResponse> UploadAsync(IFormFile file, int userId);
    }
}
