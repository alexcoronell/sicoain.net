

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class OccupationalRiskAdministratorPhone : BaseEntityPhone
    {
        public required int OccupationalRiskAdministratorId { get; set; }

        public required OccupationalRiskAdministrator OccupationalRiskAdministrator { get; set; }
    }
}
