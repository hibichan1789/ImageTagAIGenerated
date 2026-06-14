using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Infrastructure.Repositories
{
    public class FileTagRepository:IFileTagRepository
    {
        private readonly ILogger<FileTagRepository> _logger;
        private readonly MyContext _db;

        public FileTagRepository(ILogger<FileTagRepository> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task AddAsync(int fileId, int tagId, int cssStyleId)
        {
            var exists = await _db.FileTags
                .AnyAsync(ft => ft.FileId == fileId && ft.TagId == tagId && ft.CssStyleId == cssStyleId);

            if (exists)
            {
                _logger.LogInformation("すでにFileTagレコードが存在しているためDBへの追加はしません\n" +
                    "FileId={fileId}, TagId={TagId}, CssStyleId={CssStyleId}", fileId, tagId, cssStyleId);
                return;
            }
            var fileTag = new FileTag
            {
                FileId = fileId,
                TagId = tagId,
                CssStyleId = cssStyleId
            };

            _db.FileTags.Add(fileTag);
            await _db.SaveChangesAsync();
            _logger.LogInformation("FileTagを追加しました" +
                "FileId={fileId}, TagId={TagId}, CssStyleId={CssStyleId}", fileId, tagId, cssStyleId);
        }
    }
}
