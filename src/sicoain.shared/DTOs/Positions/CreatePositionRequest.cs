using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Positions
{
    public record CreatePositionRequest
    {
        [Required]
        public required string Name { get; init; }

        public string? Description { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int DepartmentId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int RiskClassId { get; init; }
    }
}
