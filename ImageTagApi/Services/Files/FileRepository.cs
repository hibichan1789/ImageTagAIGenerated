using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Services.Files
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

        public async Task<DbFile?> GetByIdAsync(int id, int userId)
        {
            return await _db.Files.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task<IEnumerable<DbFile>> GetByUserIdAsync(int userId)
        {
            return await _db.Files
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
