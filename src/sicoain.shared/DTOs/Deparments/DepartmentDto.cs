namespace sicoain.shared.DTOs.Deparments
{
    public record DepartmentDto : BaseDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
