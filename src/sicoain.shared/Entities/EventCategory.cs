using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class EventCategory : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column("level_of_severity", TypeName = "varchar(50)")]
        [Required]
        public required string LevelOfSeverity { get; set; }

        public bool RequiresHospitalization { get; set; }
    }
}
