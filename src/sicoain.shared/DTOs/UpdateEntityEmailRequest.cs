using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public record UpdateEntityEmailRequest
    {
        [Required, EmailAddress]
        public required string? Email { get; init; }
    }
}
