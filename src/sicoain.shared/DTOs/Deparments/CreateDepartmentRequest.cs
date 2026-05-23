using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Deparments
{
    public record CreateDepartmentRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }

        [MaxLength(250)]
        public string? Description { get; init; }

        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
