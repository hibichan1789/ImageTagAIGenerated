using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using ImageTagApi.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Infrastructure.Repositories
{
    public class CssStyleRepository:ICssStyleRepository
    {
        private readonly ILogger<CssStyleRepository> _logger;
        private readonly MyContext _db;

        public CssStyleRepository(ILogger<CssStyleRepository> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<CssStyle> GetOrCreateAsync(string bgColor)
        {
            // bgColorが適切なものであればそのまま返し、不適切なものであればbg-gray-700を返す
            var color = TailWindColorMapper.ValidateBgColor(bgColor);

            var style = await _db.CssStyles.FirstOrDefaultAsync(c => c.BgColor == color);
            if(style != null)
            {
                _logger.LogInformation("すでにCssStyleが存在しているためDBへの追加はしません: bgColor={color}", color);
                return style;
            }

            style = new CssStyle
            {
                BgColor = color,
                TailwindColor = TailWindColorMapper.ToEnum(color)
            };

            _db.CssStyles.Add(style);
            await _db.SaveChangesAsync();
            _logger.LogInformation("CssStyleを追加しました: TagId={TagId}, bgColor={color}", style.Id, style.BgColor);
            return style;
        }


    }
}
