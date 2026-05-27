using sicoain.shared.Entities;

namespace sicoain.shared.DTOs.EmployeeContacts
{
    public record EmployeeContactDto : BaseDto
    {
        public string? Fullname { get; init; }
        public string? Relationship { get; init; }
        public int EmployeeId { get; init; }
        public string? EmployeeFullname { get; init; }
    }
}
