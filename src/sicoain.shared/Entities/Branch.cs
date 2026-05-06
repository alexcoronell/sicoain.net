using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class Branch : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column("address_street", TypeName = "varchar(200)")]
        [MaxLength(200)]
        public string? AddressStreet { get; set; }

        public required int BusinessId { get; set; }

        public required Business Business { get; set; }

        public ICollection<BranchPhone>? Phones { get; }
        public ICollection<BranchEmail>? Emails { get; }

        public ICollection<Employee>? Employees { get; }
    }
}
