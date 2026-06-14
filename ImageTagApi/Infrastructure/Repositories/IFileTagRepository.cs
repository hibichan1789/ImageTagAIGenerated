namespace ImageTagApi.Infrastructure.Repositories
{
    public interface IFileTagRepository
    {
        Task AddAsync(int fileId, int tagId, int cssStyleId);
    }
}
