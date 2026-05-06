using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeContactPhone : BaseEntity
    {
        [Required]
        public required string Phone { get; set; }

        [Required]
        public required int EmployeeContactId { get; set; }
        public required EmployeeContact EmployeeContact { get; set; }
    }
}
