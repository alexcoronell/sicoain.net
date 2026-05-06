

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class OccupationalRiskAdministratorPhone: BaseEntity
    {
        [Required]
        public required string Phone { get; set; }

        public required int OccupationalRiskAdministratorId { get; set; }

        public required OccupationalRiskAdministrator OccupationalRiskAdministrator { get; set; }
    }
}
