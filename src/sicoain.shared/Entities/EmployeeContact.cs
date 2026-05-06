using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class EmployeeContact : BaseEntity
    {
        [Required]
        public required string Fullname { get; set; }

        [Required]
        public required string Relationship { get; set; }

        [Required]
        public required int EmployeeId { get; set; }

        public required Employee Employee { get; set; }

        public ICollection<EmployeeContactPhone>? EmployeeContactPhones { get; }
        public ICollection<EmployeeContactEmail>? EmployeeContactEmails { get; }
    }
}
