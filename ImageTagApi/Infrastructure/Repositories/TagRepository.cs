using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Infrastructure.Repositories
{
    public class TagRepository:ITagRepository
    {
        private readonly ILogger<TagRepository> _logger;
        private readonly MyContext _db;
        public TagRepository(ILogger<TagRepository> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<Tag> GetOrCreateAsync(string value)
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Value == value);
            if(tag != null)
            {
                _logger.LogInformation("すでにタグが存在しているためDBへの追加はしません: Value={Value}", value);
                return tag;
            }

            tag = new Tag { Value = value };
            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();
            _logger.LogInformation("タグを追加しました: TagId={TagId}, Value={Value}", tag.Id, tag.Value);
            return tag;
        }
    }
}
