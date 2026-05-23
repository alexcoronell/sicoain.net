

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class Position : BaseEntity
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public required int DepartmentId { get; set; }

        public required Department Department { get; set; }

        public required int RiskClassId { get; set; }

        public RiskClass? RiskClass { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
