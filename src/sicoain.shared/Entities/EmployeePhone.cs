using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeePhone : BaseEntityPhone
    {
        public required int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
