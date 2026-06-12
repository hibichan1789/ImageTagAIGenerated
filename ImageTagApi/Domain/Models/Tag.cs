using System.ComponentModel.DataAnnotations;

namespace ImageTagApi.Domain.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty; // 例: 犬, 車

        public ICollection<FileTag> FileTags { get; set; } = new List<FileTag>();
    }
}
