using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeContactEmail : BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required int EmployeeContactId { get; set; }
        public required EmployeeContact EmployeeContact { get; set; }
    }
}
