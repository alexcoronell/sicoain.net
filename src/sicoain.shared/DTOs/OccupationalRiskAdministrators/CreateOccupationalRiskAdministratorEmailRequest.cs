using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.OccupationalRiskAdministrators
{
    public record CreateOccupationalRiskAdministratorEmailRequest : CreateEntityEmailRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int OccupationalRiskAdministratorId { get; init; }
    }
}
