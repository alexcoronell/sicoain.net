using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class Witness : BaseEntity
    {

        [Required]
        public required int AccidentId { get; set; }

        [Required]
        public required Accident Accident { get; set; }

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public string? WitnessName { get; set; }

        public string? WitnessContact { get; set; }

        public required string Statement { get; set; }
    }
}
