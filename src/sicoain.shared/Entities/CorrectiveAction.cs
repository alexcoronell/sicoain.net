using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public class CorrectiveAction : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; } = string.Empty;

        [Column("due_date", TypeName = "datetime2")]
        public DateTime? DueDate { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required StatusAction Status { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required Priority Priority { get; set; }

        [Column("completion_date", TypeName = "datetime2")]
        public DateTime? CompletionDate { get; set; }

        public string? VerificationNotes { get; set; } = string.Empty;

        [Column("is_effective", TypeName = "bit")]
        public bool IsEffective { get; set; }

        [Required]
        public int AccidentId { get; set; }

        /********** Collections **********/
        public ICollection<CorrectiveActionTracking>? Trackings { get; set; }

        public Accident? Accident { get; set; }
    }
}
