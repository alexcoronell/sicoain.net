using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class Department : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
