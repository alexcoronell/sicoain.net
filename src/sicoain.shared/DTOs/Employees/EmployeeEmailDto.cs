namespace sicoain.shared.DTOs.Employees
{
    public record EmployeeEmailDto : EntityEmailDto
    {
        public required int EmployeeId { get; init; }
    }
}
