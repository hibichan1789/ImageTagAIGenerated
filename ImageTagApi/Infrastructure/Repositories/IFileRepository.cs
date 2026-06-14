using ImageTagApi.Domain.Enums;
using ImageTagApi.Domain.Models;

namespace ImageTagApi.Infrastructure.Repositories
{
    public interface IFileRepository
    {
        Task<DbFile> AddAsync(DbFile file);
        Task UpdateStatusAsync(int id, FileStatus status);
        Task<DbFile?> GetByIdAsync(int id);
        Task<IEnumerable<DbFile>> GetByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
