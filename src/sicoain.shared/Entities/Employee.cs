using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [Column("document_type", TypeName = "varchar(100)")]
        public required DocumentType DocumentType { get; set; }

        [Required]
        [Column("document_number", TypeName = "varchar(100)")]
        public required string DocumentNumber { get; set; }

        [Required]
        [Column("first_name", TypeName = "varchar(100)")]
        public required string FirstName { get; set; }

        [Column("second_name", TypeName = "varchar(100)")]
        public required string SecondName { get; set; }

        [Required]
        [Column("surname", TypeName = "varchar(100)")]
        public required string Surname { get; set; }

        [Column("second_surname", TypeName = "varchar(100)")]
        public required string SecondSurname { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string State { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string Municipality { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string Neighborhood { get; set; }

        [Required]
        [Column("address_street", TypeName = "varchar(200)")]
        public required string AddressStreet { get; set; }

        [Column("alternative_address_street", TypeName = "varchar(200)")]
        public string? AlternativeAddressStreet { get; set; }

        [Column("postal_code", TypeName = "varchar(20)")]
        public string? PostalCode { get; set; }

        [Column("hiring_date", TypeName = "datetime")]
        public DateTime? HiringDate { get; set; }

        [Column("termination_date", TypeName = "datetime")]
        public DateTime? TerminationDate { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? Diseases { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? Medications { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? Allergies { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Notes { get; set; }

        public required int BusinessId { get; set; }
        public Business? Business { get; set; }

        public required int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public required int HealthPromotionEntityId { get; set; }
        public HealthPromotionEntity? HealthPromotionEntity { get; set; }

        public required int OccupationalRiskAdministratorId { get; set; }
        public OccupationalRiskAdministrator? OccupationalRiskAdministrator { get; set; }

        public required int PositionId { get; set; }
        public Position? Position { get; set; }

        /********** Collections **********/
        public ICollection<EmployeePhone>? EmployeePhones { get; }
        public ICollection<EmployeeEmail>? EmployeeEmails { get; }

        public ICollection<EmployeeContact>? EmployeeContacts { get; }

        public ICollection<Witness>? Witnesses { get; }

    }
}
