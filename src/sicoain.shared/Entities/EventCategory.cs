using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public class EventCategory : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required AccidentSeverity LevelOfSeverity { get; set; }

        public bool RequiresHospitalization { get; set; }
    }
}
