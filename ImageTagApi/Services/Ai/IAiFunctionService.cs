using ImageTagApi.DTOs.Ai;

namespace ImageTagApi.Services.Ai
{
    public interface IAiFunctionService
    {
        Task<AiTagResponse?> GenerateTagAsync(string aiProcessedFileUrl);
    }
}
