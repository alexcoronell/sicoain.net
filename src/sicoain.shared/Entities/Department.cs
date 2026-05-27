using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class Department : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public ICollection<Position>? Positions { get; set; }
    }
}
