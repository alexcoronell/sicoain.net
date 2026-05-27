using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.EmployeeContacts
{
    public record CreateEmployeeContactPhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public required int EmployeeContactId { get; init; }
    }
}
