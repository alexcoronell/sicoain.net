

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class OccupationalRiskAdministratorEmail: BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        public required int OccupationalRiskAdministratorId { get; set; }

        public required OccupationalRiskAdministrator OccupationalRiskAdministrator { get; set; }
    }
}
