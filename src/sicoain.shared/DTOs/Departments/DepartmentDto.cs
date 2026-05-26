namespace sicoain.shared.DTOs.Departments
{
    public record DepartmentDto : BaseDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
