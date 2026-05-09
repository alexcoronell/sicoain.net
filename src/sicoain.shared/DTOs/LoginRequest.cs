using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public required string Password { get; set; }
    }
}
