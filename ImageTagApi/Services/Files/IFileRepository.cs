using ImageTagApi.Domain.Models;

namespace ImageTagApi.Services.Files
{
    public interface IFileRepository
    {
        Task<DbFile> AddAsync(DbFile file);
        Task<DbFile?> GetByIdAsync(int id, int userId);
        Task<IEnumerable<DbFile>> GetByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
