namespace sicoain.shared.DTOs.Accident
{
    public record UpdateAccidentRequest
    {
        public DateTime? EventDate { get; init; }
        public string? Description { get; init; }
        public int? EmployeeId { get; init; }
        public int? AccidentTypeId { get; init; }
        public int? EventCategoryId { get; init; }
    }
}
