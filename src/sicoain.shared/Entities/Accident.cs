using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class Accident : BaseEntity
    {
        [Column("event_date", TypeName = "datetime")]
        [Required]
        public DateTime EventDate { get; set; }

        [Column("description", TypeName = "nvarchar(500)")]
        [Required]
        public required string Description { get; set; }

        [Required]
        public required int EmployeeId { get; set; }

        public required Employee Employee { get; set; }

        public required int AccidentTypeId { get; set; }
        public required AccidentType AccidentType { get; set; }

        public required int EventCategoryId { get; set; }
        public required EventCategory EventCategory { get; set; }

        /********** Collections **********/

        public ICollection<DigitalEvidence>? DigitalEvidences { get; set; }

        public ICollection<Witness>? Witnesses { get; set; }

        public ICollection<CorrectiveAction>? CorrectiveActions { get; set; }
    }
}
