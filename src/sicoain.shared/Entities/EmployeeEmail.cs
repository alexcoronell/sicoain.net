using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeEmail : BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        public required int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
