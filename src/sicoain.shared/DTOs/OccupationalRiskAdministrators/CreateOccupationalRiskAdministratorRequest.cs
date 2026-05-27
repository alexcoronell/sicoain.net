using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.OccupationalRiskAdministrators
{
    public record CreateOccupationalRiskAdministratorRequest : BaseDto
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }
        public string? AddressStreet { get; init; }
    }
}
