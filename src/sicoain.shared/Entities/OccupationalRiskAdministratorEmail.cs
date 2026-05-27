

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class OccupationalRiskAdministratorEmail : BaseEntityEmail
    {
        public required int OccupationalRiskAdministratorId { get; set; }

        public required OccupationalRiskAdministrator OccupationalRiskAdministrator { get; set; }
    }
}
