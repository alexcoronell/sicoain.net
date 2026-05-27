using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required, MinLength(8), MaxLength(32)] string Password
    );
}
