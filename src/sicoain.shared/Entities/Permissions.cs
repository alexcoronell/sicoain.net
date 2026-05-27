using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class Permissions : BaseEntity
    {
        [Required, MaxLength(100)]
        public required string Name { get; set; } = string.Empty;

        [Required]
        public required string Module { get; set; } /* "Accidents", "Employees", "Reports" */

        [Required]
        public required string Action { get; set; } /* "Create", "Read", "Update", "Delete" */

        public string? Description { get; set; } /* Optional description of the permission */

    }
}
