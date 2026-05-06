using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public class AccidentType : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Description { get; set; }

        public AccidentSeverity Severity { get; set; }
    }
}
