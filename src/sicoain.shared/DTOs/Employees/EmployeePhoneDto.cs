namespace sicoain.shared.DTOs.Employees
{
    public record EmployeePhoneDto : EntityPhoneDto
    {
        public required int EmployeeId { get; init; }
    }
}
