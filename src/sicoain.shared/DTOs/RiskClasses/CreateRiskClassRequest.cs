using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.RiskClasses
{
    public record CreateRiskClassRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }

        [MaxLength(5)]
        public required string Code { get; init; }

        [Range(0.0001, 9.9999, ErrorMessage = "Contribution rate must be between 0.0001 and 9.9999.")]
        public decimal ContributionRate { get; init; }

        public bool IsActive { get; init; }
    }
}
