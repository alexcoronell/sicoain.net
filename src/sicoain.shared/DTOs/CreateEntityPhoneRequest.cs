using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs
{
    public record CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public required string Phone { get; init; }
    }
}
