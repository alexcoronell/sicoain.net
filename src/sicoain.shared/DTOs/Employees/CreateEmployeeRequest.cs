using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.Employees
{
    public record CreateEmployeeRequest
    {
        [Required, EnumDataType(typeof(DocumentType))]
        public required DocumentType DocumentType { get; init; }

        [Required, MinLength(6), MaxLength(50)]
        public required string DocumentNumber { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public required string FirstName { get; init; }

        public string? SecondName { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public required string Surname { get; init; }

        public string? SecondSurname { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public required string State { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public required string Municipality { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public required string Neighborhood { get; init; }

        [Required, MinLength(10), MaxLength(100)]
        public required string AddressStreet { get; init; }

        public string? AlternativeAddressStreet { get; init; }

        [Required, MinLength(6), MaxLength(6)]
        public string? PostalCode { get; init; }

        [Required, DataType(DataType.Date)]
        public required DateTime HiringDate { get; init; }

        public DateTime? TerminationDate { get; init; }

        public string? Diseases { get; init; }

        public string? Medications { get; init; }

        public string? Allergies { get; init; }

        public string? Notes { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int BusinessId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int BranchId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int HealthPromotionEntityId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int OccupationalRiskAdministratorId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int DepartmentId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public required int PositionId { get; init; }
    }
}
