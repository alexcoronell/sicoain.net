using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Employees
{
    public record CreateEmployeePhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public required int EmployeeId { get; init; }
    }
}
