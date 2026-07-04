using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public record UpdateEntityEmailRequest
    {
        public int? Id { get; init; }
        [Required, EmailAddress]
        public required string? Email { get; init; }
        public bool IsMain { get; init; }
    }
}
