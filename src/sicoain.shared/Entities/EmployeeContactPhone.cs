using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeContactPhone : BaseEntityPhone
    {
        [Required]
        public required int EmployeeContactId { get; set; }
        public required EmployeeContact EmployeeContact { get; set; }
    }
}
