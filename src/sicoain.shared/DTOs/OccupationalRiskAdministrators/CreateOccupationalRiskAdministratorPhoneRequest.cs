using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.OccupationalRiskAdministrators
{
    public record CreateOccupationalRiskAdministratorPhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int OccupationalRiskAdministratorId { get; init; }
    }
}
