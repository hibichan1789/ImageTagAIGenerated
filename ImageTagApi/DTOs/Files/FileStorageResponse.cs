namespace ImageTagApi.DTOs.Files
{
    public class FileStorageResponse
    {
        public string UniqueFileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileExtension {  get; set; } = string.Empty;
    }
}
