using System.ComponentModel.DataAnnotations;

namespace ImageTagApi.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        [StringLength(maximumLength: 255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
