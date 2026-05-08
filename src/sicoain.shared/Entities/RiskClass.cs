using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class RiskClass : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Required, MaxLength(5)]
        public required string Code { get; set; }

        [Column("contribution_rate", TypeName = "decimal(5,4)")]
        public decimal ContributionRate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Position>? Positions { get; set; }

    }
}
