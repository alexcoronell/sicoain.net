namespace sicoain.shared.DTOs.Positions
{
    public record PositionDto : BaseDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public int DepartmentId { get; init; }
        public int RiskClassId { get; init; }
    }
}
