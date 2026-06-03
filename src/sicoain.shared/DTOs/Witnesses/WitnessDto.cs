namespace sicoain.shared.DTOs.Witnesses
{
    public record WitnessDto : BaseDto
    {
        public int AccidentId { get; init; }
        public int EmployeeId { get; init; }
        public string? EmployeeFullname { get; init; }
        public string? WitnessName { get; init; }
        public string? WitnessContact { get; init; }
        public string? Statement { get; init; }
    }
}
