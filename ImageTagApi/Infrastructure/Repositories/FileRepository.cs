using ImageTagApi.Context;
using ImageTagApi.Domain.Enums;
using ImageTagApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Infrastructure.Repositories
{
    public class FileRepository:IFileRepository
    {
        private readonly ILogger<FileRepository> _logger;
        private readonly MyContext _db;

        public FileRepository(ILogger<FileRepository> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<DbFile> AddAsync(DbFile file)
        {
            _logger.LogInformation("FileName : {fileName}をDBに追加します", file.OriginalFileName);

            await _db.Files.AddAsync(file);
            return file;
        }

        public async Task UpdateStatusAsync(int id, FileStatus status)
        {
            var file = await _db.Files.FindAsync(id);
            if(file == null)
            {
                _logger.LogWarning("ファイルのレコードが見つかりません: FileId={FileId}", id);
                return;
            }

            file.Status = status;
            await SaveChangesAsync();
        }

        public async Task<DbFile?> GetByIdAsync(int id)
        {
            return await _db.Files
                .Include(f => f.FileTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.FileTags)
                .ThenInclude(f => f.CssStyle)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<DbFile>> GetByUserIdAsync(int userId)
        {
            return await _db.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.FileTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.FileTags)
                .ThenInclude(f => f.CssStyle)
                .OrderBy(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
