using ImageTagApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ImageTagApi.Domain.Models
{
    public class CssStyle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:50)]
        public string BgColor { get; set; } = string.Empty; // "bg-red-700"など

        [Required]
        public TailWindColor TailwindColor { get; set; } = TailWindColor.Gray; // enumも保存することで後々の拡張がしやすいかも


        public ICollection<FileTag> FileTags { get; set; } = new List<FileTag>(); 
    }
}
