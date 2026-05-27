using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.Employees
{
    public record EmployeeDto : BaseDto
    {
        public DocumentType DocumentType { get; init; }
        public string DocumentNumber { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string? SecondName { get; init; }
        public string Surname { get; init; } = string.Empty;
        public string? SecondSurname { get; init; }
        public string State { get; init; } = string.Empty;

        public string Municipality { get; init; } = string.Empty;
        public string Neighborhood { get; init; } = string.Empty;
        public string AddressStreet { get; init; } = string.Empty;
        public string? AlternativeAddressStreet { get; init; }
        public string? PostalCode { get; init; }
        public DateTime HiringDate { get; init; }
        public DateTime? TerminationDate { get; init; }
        public string? Diseases { get; init; }
        public string? Medications { get; init; }
        public string? Allergies { get; init; }
        public string? Notes { get; init; }
        public int BusinessId { get; init; }
        public string BusinessName { get; init; } = string.Empty;
        public int BranchId { get; init; }
        public string BranchName { get; init; } = string.Empty;
        public int HealthPromotionEntityId { get; init; }
        public string HealthPromotionEntityName { get; init; } = string.Empty;
        public int OccupationalRiskAdministratorId { get; init; }
        public string OccupationalRiskAdministratorName { get; init; } = string.Empty;
        public int DepartmentId { get; init; }
        public string DepartmentName { get; init; } = string.Empty;
        public int PositionId { get; init; }
        public string PositionName { get; init; } = string.Empty;
    }
}
