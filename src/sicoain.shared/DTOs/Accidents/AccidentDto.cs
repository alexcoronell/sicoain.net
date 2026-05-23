namespace sicoain.shared.DTOs.Accident
{
    public record AccidentDto : BaseDto
    {
        public DateTime EventDate { get; init; }
        public string Description { get; init; } = string.Empty;

        public int EmployeeId { get; init; }
        public string EmployeeFullname { get; init; } = string.Empty;

        public int AccidentTypeId { get; init; }
        public string AccidentTypeName { get; init; } = string.Empty;

        public int EventCategoryId { get; init; }
        public string EventCategoryName { get; init; } = string.Empty;
    }
}
