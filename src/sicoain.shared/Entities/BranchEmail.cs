using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BranchEmail : BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        public required int BranchId { get; set; }

        public required Branch Branch { get; set; }
    }
}

