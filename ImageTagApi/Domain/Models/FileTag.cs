namespace ImageTagApi.Domain.Models
{
    public class FileTag
    {
        // TagとCssStyleの中間テーブル

        public int FileId { get; set; }
        public DbFile File { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public int CssStyleId { get; set; }
        public CssStyle CssStyle { get; set; } = null!;
    }
}
