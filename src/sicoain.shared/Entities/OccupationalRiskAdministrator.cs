using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class OccupationalRiskAdministrator : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        [Column("address_street", TypeName = "varchar(200)")]
        [MaxLength(200)]
        public string? AddressStreet { get; set; }

        public ICollection<OccupationalRiskAdministratorPhone>? Phones { get; set; }
        public ICollection<OccupationalRiskAdministratorEmail>? Emails { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
