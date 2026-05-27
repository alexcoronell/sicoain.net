using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeContactEmail : BaseEntityEmail
    {
        [Required]
        public required int EmployeeContactId { get; set; }
        public required EmployeeContact EmployeeContact { get; set; }
    }
}
