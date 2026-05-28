namespace sicoain.shared.DTOs.EmployeeContacts
{
    public record UpdateEmployeeContactRequest
    {
        public string? Fullname { get; init; }
        public string? Relationship { get; init; }
    }
}
