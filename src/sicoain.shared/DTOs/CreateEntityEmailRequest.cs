using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public record CreateEntityEmailRequest
    {
        [Required, EmailAddress]
        public required string Email { get; init; }
        public bool IsMain { get; init; }
    }
}
