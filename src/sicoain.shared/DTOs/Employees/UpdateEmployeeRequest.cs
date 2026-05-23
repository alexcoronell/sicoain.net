using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.Employees
{
    public record UpdateEmployeeRequest
    {
        public DocumentType? DocumentType { get; init; }
        public string? DocumentNumber { get; init; }
        public string? FirstName { get; init; }
        public string? SecondName { get; init; }
        public string? Surname { get; init; }
        public string? SecondSurname { get; init; }
        public string? State { get; init; }
        public string? Municipality { get; init; }
        public string? Neighborhood { get; init; }
        public string? AddressStreet { get; init; }
        public string? AlternativeAddressStreet { get; init; }
        public string? PostalCode { get; init; }
        public DateTime? HiringDate { get; init; }
        public DateTime? TerminationDate { get; init; }
        public string? Diseases { get; init; }
        public string? Medications { get; init; }
        public string? Allergies { get; init; }
        public string? Notes { get; init; }
        public int? BusinessId { get; init; }
        public int? BranchId { get; init; }
        public int? HealthPromotionEntityId { get; init; }
        public int? OccupationalRiskAdministratorId { get; init; }
        public int? PositionId { get; init; }
    }
}
