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

        public ICollection<BusinessPhone>? Phones { get; set; }
        public ICollection<BusinessEmail>? Emails { get; set; }

        public ICollection<Branch>? Branches { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
