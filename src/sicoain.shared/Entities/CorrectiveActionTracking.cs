using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class CorrectiveActionTracking : BaseEntity
    {
        [Required]
        public required int CorrectiveActionId { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public required string OldStatus { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public required string NewStatus { get; set; }

        [Required]
        public required DateTime TrackingDate { get; set; }

        [Required]
        public required string Comments { get; set; } = string.Empty;
    }
}
