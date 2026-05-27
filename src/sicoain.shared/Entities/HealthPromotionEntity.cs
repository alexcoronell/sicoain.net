using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class HealthPromotionEntity : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column("address_street", TypeName = "varchar(200)")]
        [MaxLength(200)]
        public string? AddressStreet { get; set; }

        public string? Notes { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
