using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.EmployeeContacts
{
    public record CreateEmployeeContactRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string? Fullname { get; init; }

        [Required, MinLength(3), MaxLength(100)]
        public string? Relationship { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int EmployeeId { get; init; }
    }
}
