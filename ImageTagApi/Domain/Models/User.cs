using System.ComponentModel.DataAnnotations;

namespace ImageTagApi.Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        // DateTimeOffset はDateTimeとは違いUTCとの差も保存するため異なるグローバル環境への対応ができる

        public ICollection<DbFile> Files { get; set; } = new List<DbFile>();
    }
}
