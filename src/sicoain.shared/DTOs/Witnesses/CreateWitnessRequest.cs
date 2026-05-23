using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Witnesses
{
    public record CreateWitnessRequest
    {
        [Required, Range(1, int.MaxValue)]
        public required int AccidentId { get; init; }

        [Range(1, int.MaxValue)]
        public int? EmployeeId { get; init; }

        public string? WitnessName { get; init; }
        public string? WitnessContact { get; init; }
        
        [Required, MinLength(50), MaxLength(500)]
        public required string Statement { get; init; }
    }
}