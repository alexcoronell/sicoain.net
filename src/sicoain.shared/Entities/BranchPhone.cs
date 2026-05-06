using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BranchPhone: BaseEntity
    {
        [Required]
        public required string Phone { get; set; }

        public required int BranchId { get; set; }

        public required Branch Branch { get; set; }
    }
}
