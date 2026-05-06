using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class Business : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column("address_street", TypeName = "varchar(200)")]
        [MaxLength(200)]
        public string? AddressStreet { get; set; }

        public ICollection<BusinessPhone>? Phones { get; }
        public ICollection<BusinessEmail>? Emails { get; }

        public ICollection<Branch>? Branches { get; }

        public ICollection<Employee>? Employees { get; }
    }
}
